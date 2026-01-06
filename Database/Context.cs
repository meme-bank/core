using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace OctopusAPI.Database
{
  public class MeduzaContext : DbContext
  {
    // -- Database Connection -- //
    private string User = "meduza";
    private string DatabaseName = "meduza";
    private string Host = "127.0.0.1";
    private string Password = "meduza";
    private int Port = 5432;

    // -- Entities -- //
    // Core Entities //
    public DbSet<Entities.Core.Photo> Photos { get; set; }
    public DbSet<Entities.Core.Setting> Settings { get; set; }

    // Economic Base Entities //
    public DbSet<Entities.Economic.Wallet> Wallets { get; set; }
    public DbSet<Entities.Economic.TransferNote> TransferNotes { get; set; }
    public DbSet<Entities.Economic.Currency> Currencies { get; set; }

    // Trading Entities //
    // Material Items
    public DbSet<Entities.Trading.Item> Items { get; set; }
    public DbSet<Entities.Trading.ItemBlueprint> ItemBlueprints { get; set; }
    public DbSet<Entities.Trading.Product> Products { get; set; }

    // Services
    public DbSet<Entities.Trading.Service> Services { get; set; }
    public DbSet<Entities.Trading.ProvideService> ProvideServices { get; set; }

    // Categories of Services, Tariffs and Items
    public DbSet<Entities.Trading.Category> Categories { get; set; }

    // Blog Entities //
    public DbSet<Entities.Blog.Post> Posts { get; set; }
    public DbSet<Entities.Blog.Blog> Blogs { get; set; }

    // Loan Entities //
    public DbSet<Entities.Loan.Loan> Loans { get; set; }
    public DbSet<Entities.Loan.Deposit> Deposits { get; set; }

    // -- Constructor -- //
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
          .UseNpgsql($"Server={Host};Port={Port};Database={DatabaseName};User Id={User};Password={Password}")
          .UseSnakeCaseNamingConvention();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      SetupRelationships(modelBuilder);
      SetupProperties(modelBuilder);
      SeedInitialData(modelBuilder);
    }

    private void SetupProperties(ModelBuilder modelBuilder)
    {
      foreach (var property in modelBuilder.Model.GetEntityTypes()
      .SelectMany(t => t.GetProperties())
      .Where(p => p.ClrType == typeof(decimal)))
      {
        property.SetPrecision(18);
        property.SetScale(4);
      }
    }

    private void SetupRelationships(ModelBuilder modelBuilder)
    {
      // --- Many-to-many ---
      // Wallet <-> TransferNote
      modelBuilder.Entity<Entities.Economic.Wallet>()
          .HasMany(w => w.RecivedTransferNotes)
          .WithOne(tn => tn.Receiver)
          .HasForeignKey(tn => tn.ReceiverId)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<Entities.Economic.Wallet>()
          .HasMany(w => w.SentTransferNotes)
          .WithOne(tn => tn.Sender)
          .HasForeignKey(tn => tn.SenderId)
          .OnDelete(DeleteBehavior.Restrict);

      // Service, Items <-> Category
      modelBuilder.Entity<Entities.Trading.Service>()
          .HasMany(s => s.Categories)
          .WithMany(c => c.Services);

      modelBuilder.Entity<Entities.Trading.ItemBlueprint>()
          .HasMany(p => p.Categories)
          .WithMany(c => c.ItemBlueprints);

      // ItemBlueprint <-> ItemBlueprint (Recipe)
      modelBuilder.Entity<Entities.Trading.ItemBlueprintRecipe>(entity =>
      {
        // Составной ключ, чтобы один и тот же ингредиент не дублировался в одном рецепте
        entity.HasKey(r => new { r.RecipeItemId, r.CraftItemId });

        entity.HasOne(r => r.RecipeItem)
                  .WithMany(b => b.CraftableItems) // Где этот предмет используется как сырье
                  .HasForeignKey(r => r.RecipeItemId);

        entity.HasOne(r => r.CraftItem)
                  .WithMany(b => b.Recipe) // Из чего состоит этот предмет
                  .HasForeignKey(r => r.CraftItemId);
      });
    }

    private void SeedInitialData(ModelBuilder modelBuilder)
    {
      var leuroCurrencyId = "LMC";
      var bankId = 3; // НБМ
      var fallelandId = 2; // Ловушкинск
      var adminId = 1; // Артемос, то бишь я
      var bankWalletId = Guid.Parse("00000000-0000-0000-0000-000000000001");
      var fallelandWalletId = Guid.Parse("00000000-0000-0000-0000-000000000002");
      var adminWalletId = Guid.Parse("00000000-0000-0000-0000-000000000123");
      var defaultPhotoId = "default_currency_photo";

      modelBuilder.Entity<Entities.Core.Photo>().HasData(new
      {
        Id = defaultPhotoId,
        OwnerId = bankId,
        Image = new byte[0], // Пустой массив для инициализации
        RequestedAt = DateTime.UtcNow,
        UploadedAt = DateTime.UtcNow,
        Type = Entities.Core.PhotoType.Icon // Или другой подходящий тип
      });

      modelBuilder.Entity<Entities.Economic.Currency>().HasData(new
      {
        Id = leuroCurrencyId,
        Name = "Левро",
        ExchangeRate = 1.0m, // Эталон
        EmmissionCountryId = fallelandId, // Ловушкинск
        CreatedAt = new DateTime(2020, 8, 25, 0, 0, 0, DateTimeKind.Utc),
        MonochromePhotoId = "default_currency_photo"
      });

      modelBuilder.Entity<Entities.Economic.Wallet>().HasData(new
      {
        Id = bankWalletId,
        Name = "Резерв НБМ",
        Description = "Главный кошелек для сбора банковских комиссий и эмиссии",
        CurrencyId = leuroCurrencyId,
        OwnerId = bankId,
        Balance = 1000000000m, // Начальный капитал банка
        CreatedAt = new DateTime(2020, 8, 25, 0, 0, 0, DateTimeKind.Utc)
      });

      modelBuilder.Entity<Entities.Economic.Wallet>().HasData(new
      {
        Id = fallelandWalletId,
        Name = "Резерв Ловушкинска",
        Description = "Главный кошелек для сбора налогов",
        CurrencyId = leuroCurrencyId,
        OwnerId = fallelandId,
        Balance = 1_000_000_000_000m,
        CreatedAt = new DateTime(2020, 8, 25, 0, 0, 0, DateTimeKind.Utc)
      });

      modelBuilder.Entity<Entities.Economic.Wallet>().HasData(new
      {
        Id = adminWalletId,
        Name = "Личный кошелек",
        Description = "Личный кошелек администратора системы",
        CurrencyId = leuroCurrencyId,
        OwnerId = adminId,
        Balance = 1_000_000_000_000m,
        CreatedAt = new DateTime(2020, 8, 25, 0, 0, 0, DateTimeKind.Utc)
      });

      modelBuilder.Entity<Entities.Trading.Category>().HasData(new
      {
        Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
        Name = "Банкинг",
        Description = "Услуги, связанные с банковскими операциями и финансовыми сервисами."
      });

      modelBuilder.Entity<Entities.Trading.Service>().HasData(new
      {
        CurrencyId = leuroCurrencyId,
        Id = Guid.Parse("00000000-0000-0000-0000-00000000A001"),
        Name = "НБМ Prime",
        Description = "Премиум подписка на эксклюзивные банковские услуги от Народного Банка Мемов.",
        ProviderId = bankId,
        PublishedAt = DateTime.UtcNow,
        Type = Entities.Trading.ServiceType.Subscription,
        Duration = TimeSpan.FromDays(30),
        IsOtherActivate = true
      });

      modelBuilder.Entity<Entities.Trading.Service>()
          .HasMany(s => s.Categories)
          .WithMany(c => c.Services)
          .UsingEntity(j => j.HasData(new
          {
            ServicesId = Guid.Parse("00000000-0000-0000-0000-00000000A001"),
            CategoriesId = Guid.Parse("00000000-0000-0000-0000-000000000002")
          }));
    }
  }
}