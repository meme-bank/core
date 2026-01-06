using System.ComponentModel.DataAnnotations;
using OctopusAPI.Database.Entities.Economic;

public class Stock
{
  [Required]
  [Key]
  public string Ticker { get; set; } // Например, "NBM" или "LVK"
  public int IssuerId { get; set; } // Компания, которая выпустила акции

  public decimal TotalSupply { get; set; } // Общее кол-во выпущенных акций
  public decimal CurrentPrice { get; set; }


  public Currency Currency { get; set; }
  public string CurrencyId { get; set; }
}