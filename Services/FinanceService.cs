using Microsoft.EntityFrameworkCore;
using OctopusAPI.Database;
using OctopusAPI.Database.Entities.Economic;
using OctopusAPI.Database.Entities.Loan;

namespace OctopusAPI.Services
{
  public class FinanceService
  {
    private readonly EconomyService _economyService;
    private readonly MeduzaContext _context;

    private readonly Guid _bankWalletId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public FinanceService(EconomyService economyService, MeduzaContext context)
    {
      _economyService = economyService;
      _context = context;
    }

    public async Task<Deposit> OpenDepositAsync(Guid walletId, decimal amount, string currencyId)
    {
      var wallet = await _context.Wallets.FindAsync(walletId);
      if (wallet == null)
        throw new Exception("Кошелёк не найден.");

      using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        var hasPrime = await _economyService.HasPrime(wallet.OwnerId);
        var rate = hasPrime ? 0.08m : 0.05m;

        var deposit = new Deposit
        {
          OwnerId = wallet.OwnerId,
          Amount = amount,
          CreatedAt = DateTime.UtcNow,
          CurrencyId = currencyId,
          Currency = await _context.Currencies.FindAsync(currencyId),
          Id = Guid.NewGuid(),
          InterestRate = rate, // Пример фиксированной ставки, можно изменить логику по необходимости
          IsRenewed = false,
          LastInterestAccrual = DateTime.UtcNow,
          MaturityDate = DateTime.UtcNow.AddDays(7) // Когда вклад даёт доход
        };

        await _economyService.TransferAsync(
          senderId: walletId,
          receiverId: _bankWalletId,
          amount: amount,
          currencyId: currencyId,
          note: "Открытие вклада",
          type: TransferNoteType.Buying,
          transaction: transaction
        );

        _context.Deposits.Add(deposit);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return deposit;
      }
      catch
      {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task<Loan> TakeLoanAsync(Guid walletId, decimal amount, string currencyId)
    {
      var wallet = await _context.Wallets.FindAsync(walletId);
      if (wallet == null)
        throw new Exception("Кошелёк не найден.");

      var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        var hasPrime = await _economyService.HasPrime(wallet.OwnerId);
        var interestRate = hasPrime ? 0.10m : 0.15m;

        await _economyService.TransferAsync(
          senderId: _bankWalletId,
          receiverId: walletId,
          amount: amount,
          currencyId: currencyId,
          note: "Выдача кредита",
          type: TransferNoteType.Buying
        );

        var loan = new Loan
        {
          BorrowerId = wallet.OwnerId,
          PrincipalAmount = amount,
          RemainingAmount = amount * (1 + interestRate),
          InterestRate = interestRate,
          IssuedAt = DateTime.UtcNow,
          LastInterestAccrual = DateTime.UtcNow,
          Status = LoanStatus.Approved,
          CurrencyId = currencyId,
          Currency = await _context.Currencies.FindAsync(currencyId),
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();

        return loan;
      }
      catch
      {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task<Wallet> CloseDepositAsync(Guid depositId, Guid walletId)
    {
      var deposit = await _context.Deposits.FindAsync(depositId);
      var wallet = await _context.Wallets.FindAsync(walletId);

      if (deposit == null)
        throw new Exception("Вклад не найден.");
      if (wallet == null)
        throw new Exception("Кошелёк не найден.");
      if (deposit.OwnerId != wallet.OwnerId)
        throw new Exception("Вклад не принадлежит владельцу кошелька.");

      var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        await _economyService.TransferAsync(
          senderId: _bankWalletId,
          receiverId: walletId,
          amount: deposit.Amount,
          currencyId: deposit.CurrencyId,
          note: "Закрытие вклада",
          type: TransferNoteType.Buying
        );

        _context.Deposits.Remove(deposit);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return wallet;
      }
      catch
      {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task<Loan> LoanRepaimentAsync(Guid loanId, Guid walletId, decimal amount)
    {
      var loan = await _context.Loans.FindAsync(loanId);
      var wallet = await _context.Wallets.FindAsync(walletId);

      if (loan == null)
        throw new Exception("Кредит не найден.");
      if (wallet == null)
        throw new Exception("Кошелёк не найден.");
      if (loan.BorrowerId != wallet.OwnerId)
        throw new Exception("Кредит не принадлежит владельцу кошелька.");
      if (loan.Status != LoanStatus.Approved)
        throw new Exception("Кредит не активен для погашения.");

      var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        await _economyService.TransferAsync(
          senderId: walletId,
          receiverId: _bankWalletId,
          amount: amount,
          currencyId: loan.CurrencyId,
          note: "Погашение кредита",
          type: TransferNoteType.Buying
        );

        loan.RemainingAmount -= amount;
        if (loan.RemainingAmount <= 0)
        {
          loan.Status = LoanStatus.Closed;
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return loan;
      }
      catch
      {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task ProcessFinancialAccrualsAsync()
    {
      var now = DateTime.UtcNow;

      // 1. Обработка депозитов (Начисляем доход игрокам)
      // Берём вклады, у которых пришло время начисления
      var pendingDeposits = await _context.Deposits
          .Where(d => d.MaturityDate > now && d.LastInterestAccrual.AddDays(1) <= now)
          .ToListAsync();

      foreach (var deposit in pendingDeposits)
      {
        // Считаем доход за день (процентная ставка / 12 дней)
        decimal dailyInterest = (deposit.Amount * deposit.InterestRate) / 12;
        deposit.Amount += dailyInterest;
        deposit.LastInterestAccrual = now;
      }

      // 2. Обработка кредитов (Начисляем проценты к долгу)
      var activeLoans = await _context.Loans
          .Where(l => l.Status == LoanStatus.Approved && l.LastInterestAccrual.AddDays(1) <= now)
          .ToListAsync();

      foreach (var loan in activeLoans)
      {
        // Кредитный процент капает в пользу банка
        decimal dailyDebt = (loan.PrincipalAmount * loan.InterestRate) / 12; // По РП 12 дней = 1 год
        loan.RemainingAmount += dailyDebt;
        loan.LastInterestAccrual = now;
      }

      await _context.SaveChangesAsync();
    }
  }
}