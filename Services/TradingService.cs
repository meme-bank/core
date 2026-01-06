using Microsoft.EntityFrameworkCore;
using OctopusAPI.Database;
using OctopusAPI.Database.Entities.Trading;
using OctopusAPI.Database.Entities.Economic;
using Microsoft.EntityFrameworkCore.Storage;

namespace OctopusAPI.Services
{
  public class TradingService
  {
    private readonly MeduzaContext _context;
    private readonly EconomyService _economyService;

    public TradingService(MeduzaContext context, EconomyService economyService)
    {
      _context = context;
      _economyService = economyService;
    }

    public async Task<Item> BuyItemAsync(Guid buyerWalletId, int sellerId, Guid blueprintId, decimal quantity)
    {
      var product = await _context.Products
        .Include(p => p.ItemBlueprint)
        .Where(p => p.ItemBlueprintId == blueprintId && p.SellerId == sellerId)
        .FirstOrDefaultAsync();
      var seller = await _context.Wallets.Where(w => w.OwnerId == sellerId).FirstOrDefaultAsync();
      var buyer = await _context.Wallets.FindAsync(buyerWalletId);

      if (seller == null)
        throw new Exception("Кошелёк продавца не найден.");
      if (product == null || product.Amount < quantity)
        throw new Exception("Продукт недоступен в достаточном количестве.");
      if (buyer == null)
        throw new Exception("Кошелёк покупателя не найден.");

      var verifiedQuantity = product.ItemBlueprint.MeasuredIn == MeasuredIn.Pieces
        ? Math.Floor(quantity)
        : quantity;
      var totalPrice = product.Price * verifiedQuantity;

      using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        await _economyService.TransferAsync(
          transaction: transaction,
          senderId: buyerWalletId,
          receiverId: seller.Id,
          amount: totalPrice,
          currencyId: product.CurrencyId,
          note: $"Покупка {verifiedQuantity} x {product.ItemBlueprint.Name}",
          type: TransferNoteType.Buying
        );

        var item = await _context.Items.Where(i => i.OwnerId == buyer.OwnerId && i.ItemBlueprintId == blueprintId).FirstOrDefaultAsync();
        var isCreated = item == null;

        item ??= new Item
        {
          OwnerId = buyer.OwnerId,
          ItemBlueprintId = blueprintId,
          OwnedAt = DateTime.UtcNow,
          Amount = 0m,
          ItemBlueprint = product.ItemBlueprint
        };

        item.Amount += verifiedQuantity;
        product.Amount -= verifiedQuantity;

        if (isCreated)
          _context.Items.Add(item);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return item;
      }
      catch
      {
        await transaction.RollbackAsync();
        throw;
      }



    }

    public async Task<ProvideService> ProvideServiceAsync(Guid buyerWalletId, Guid serviceId)
    {
      var service = await _context.Services.FindAsync(serviceId);
      var buyerWallet = await _context.Wallets.FindAsync(buyerWalletId);
      var providerWallet = await _context.Wallets.Where(w => w.OwnerId == service!.ProviderId).FirstOrDefaultAsync();

      if (providerWallet == null)
        throw new Exception("Кошелёк провайдера не найден.");
      if (buyerWallet == null)
        throw new Exception("Кошелёк покупателя не найден.");
      if (service == null)
        throw new Exception("Услуга не найдена.");
      if (service.IsOtherActivate)
        throw new Exception("Услуга активируется не через покупку.");

      var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        if (service.Price > 0 && service.Price != null)
          await _economyService.TransferAsync(
            transaction: transaction,
            senderId: buyerWalletId,
            receiverId: providerWallet.Id,
            amount: service.Price ?? 0m,
            currencyId: service.CurrencyId,
            note: $"Оплата услуги {service.Name}",
            type: TransferNoteType.Buying
          );

        var providedService = new ProvideService()
        {
          ServiceId = service.Id,
          BuyerId = buyerWallet.OwnerId,
          StartAt = DateTime.UtcNow,
          ExpiresAt = service.Duration != null ? DateTime.UtcNow.Add(service.Duration ?? TimeSpan.Zero) : null,
          Service = service,
          CurrencyId = service.CurrencyId,
          Currency = service.Currency,
          Price = service.Price ?? 0m,
          Status = ProvideStatus.Active
        };
        await _context.ProvideServices.AddAsync(providedService);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return providedService;
      }
      catch
      {
        await transaction.RollbackAsync();
        throw;
      }
    }
  }
}