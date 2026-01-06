using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OctopusAPI.Database;
using OctopusAPI.Database.Entities.Economic;

namespace OctopusAPI.Services
{
  public class EconomyService
  {
    private readonly MeduzaContext _context;

    private readonly Guid bankWalletId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly Guid primeServiceId = Guid.Parse("00000000-0000-0000-0000-00000000A001");

    public EconomyService(MeduzaContext context)
    {
      _context = context;
    }

    public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrencyId, string toCurrencyId)
    {
      var fromCurrency = await _context.Currencies.FindAsync(fromCurrencyId);
      var toCurrency = await _context.Currencies.FindAsync(toCurrencyId);

      if (fromCurrency == null || toCurrency == null)
      {
        throw new ArgumentException("Invalid currency ID");
      }

      if (toCurrency.ExchangeRate <= 0)
        throw new DivideByZeroException($"Курс валюты {toCurrencyId} установлен некорректно (ноль или меньше).");

      decimal amountInBase = amount * fromCurrency.ExchangeRate;
      decimal convertedAmount = amountInBase / toCurrency.ExchangeRate;

      return convertedAmount;
    }

    public async Task<bool> HasPrime(int buyerId) => await _context.ProvideServices
                            .AnyAsync(ps => ps.BuyerId == buyerId &&
                            ps.ServiceId == primeServiceId &&
                            (ps.ExpiresAt == null || ps.ExpiresAt > DateTime.UtcNow));

    public async Task<TransferNote> TransferAsync(Guid senderId, Guid receiverId, decimal amount, string currencyId, string? note, IDbContextTransaction? transaction, TransferNoteType type = TransferNoteType.Personal)
    {
      if (transaction == null)
      {
        using var newTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
          var result = await TransferAsync(senderId, receiverId, amount, currencyId, note, newTransaction, type);
          await _context.SaveChangesAsync();
          await newTransaction.CommitAsync();
          return result;
        }
        catch
        {
          await newTransaction.RollbackAsync();
          throw;
        }
      }
      var sender = await _context.Wallets.FindAsync(senderId);
      var receiver = await _context.Wallets.FindAsync(receiverId);
      var currency = await _context.Currencies.FindAsync(currencyId);

      if (sender == null || receiver == null || currency == null)
        throw new ArgumentException("Недействительный идентификатор кошелька/ов или валюты");

      // Проверка на наличие Prime и расчет комиссии
      var hasPrime = await HasPrime(sender.OwnerId);
      var feePercentage = hasPrime || type != TransferNoteType.Personal ? 0m : 0.05m; // 5% комиссия без Prime для персональных переводов
      decimal feeAmount = amount * feePercentage;
      decimal totalToSpend = amount + feeAmount;

      decimal amountInSenderCurrency = await ConvertCurrencyAsync(totalToSpend, currencyId, sender.CurrencyId);

      if (feeAmount > 0)
      {
        var bankWallet = await _context.Wallets.FindAsync(bankWalletId);
        if (bankWallet != null)
          bankWallet.Balance += await ConvertCurrencyAsync(feeAmount, currencyId, bankWallet.CurrencyId);
      }

      // Выполняем трансфер
      if (sender.Balance < amountInSenderCurrency)
        throw new ArgumentException("Недостаточно средств на кошельке отправителя");

      sender.Balance -= amountInSenderCurrency;
      receiver.Balance += await ConvertCurrencyAsync(amount, currencyId, receiver.CurrencyId);

      var transferNote = new TransferNote()
      {
        Id = Guid.NewGuid(),
        Sender = sender,
        Receiver = receiver,
        Amount = amount,
        SenderId = senderId,
        ReceiverId = receiverId,
        NotedAt = DateTime.UtcNow,
        Currency = currency,
        CurrencyId = currency.Id,
        ExchangeRateAtTransfer = currency.ExchangeRate,
        Description = note,
        Type = type,
      };

      _context.TransferNotes.Add(transferNote);
      return transferNote;
    }

    public async Task<Wallet> CreateWallet(int ownerId, string currencyId, string name)
    {
      Wallet wallet = new()
      {
        Id = Guid.NewGuid(),
        OwnerId = ownerId,
        CurrencyId = currencyId,
        Balance = 0m,
        CreatedAt = DateTime.UtcNow,
        Name = name,
        Currency = await _context.Currencies.FindAsync(currencyId) ?? throw new ArgumentException("Invalid currency ID")
      };

      _context.Wallets.Add(wallet);
      await _context.SaveChangesAsync();

      return wallet;
    }

    public async Task<Currency> CreateCurrency(string id, string name, int emmissionCountryId)
    {
      Currency currency = new()
      {
        Id = id,
        Name = name,
        EmmissionCountryId = emmissionCountryId,
        MonochromePhoto = new Database.Entities.Core.Photo
        {
          Id = Guid.NewGuid(),
          Type = Database.Entities.Core.PhotoType.Icon,
          RequestedAt = DateTime.UtcNow,
          OwnerId = emmissionCountryId
        },
        CreatedAt = DateTime.UtcNow
      };

      _context.Currencies.Add(currency);
      await _context.SaveChangesAsync();

      return currency;
    }

    internal async Task TransferAsync(Guid senderId, object receiverId, decimal amount, string currencyId, string note, object type)
    {
      throw new NotImplementedException();
    }
  }
}