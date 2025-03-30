using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OctopusAPI.Database.Entities.Economic;

namespace OctopusAPI.Database.Entities.Trading
{
    public class Product
    {
        [Key]
        public Guid ItemBlueprintId { get; set; }
        [ForeignKey("ItemBlueprintId")]
        public required ItemBlueprint ItemBlueprint { get; set; }
        public int SellerId { get; set; }
        public decimal Price { get; set; }
        public required string CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public required Currency Currency { get; set; }
        public decimal Amount { get; set; }
    }
}