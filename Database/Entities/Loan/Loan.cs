using OctopusAPI.Database.Entities.Economic;

namespace OctopusAPI.Database.Entities.Loan
{
  public enum LoanStatus
  {
    Pending,
    Approved,
    Rejected,
    Disbursed,
    Closed
  }

  public class Loan
  {
    public Guid Id { get; set; }
    public int BorrowerId { get; set; } // Кто взял кредит

    public decimal PrincipalAmount { get; set; } // Тело кредита
    public decimal RemainingAmount { get; set; } // Сколько осталось вернуть (с процентами)
    public Currency Currency { get; set; }
    public string CurrencyId { get; set; }

    public decimal InterestRate { get; set; } // Ставка (например, 0.15 для 15%)
    public DateTime IssuedAt { get; set; }
    public DateTime LastInterestAccrual { get; set; } // Когда последний раз капал процент
    public LoanStatus Status { get; set; }
  }
}