using OctopusAPI.Database.Entities.Economic;

namespace OctopusAPI.Database.Entities.Loan
{
  public class Deposit
  {
    public Guid Id { get; set; }
    public int OwnerId { get; set; } // Владелец депозита
    public decimal Amount { get; set; } // Сумма депозита
    public Currency Currency { get; set; }
    public string CurrencyId { get; set; }
    public decimal InterestRate { get; set; } // Процентная ставка (например, 0.05 для 5%)
    public DateTime CreatedAt { get; set; } // Когда создан депозит
    public DateTime LastInterestAccrual { get; set; } // Когда последний раз капал процент
    public DateTime MaturityDate { get; set; } // Когда депозит созревает
    public bool IsRenewed { get; set; } // Продлен ли депозит автоматически
  }
}