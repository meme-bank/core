using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OctopusAPI.Database.Entities.Core;

namespace OctopusAPI.Database.Entities.Economic
{
    public class Currency
    {
        [Required]
        [Key]
        public required string Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required Photo MonochromePhoto { get; set; } // symbol
        public decimal ExchangeRate { get; set; } = 100; // against Leuro or another base currency (BC - Base Currency, AC - Alternative Currency, 1AC = 100BC => 0.01, 100AC = 1BC => 1.0)
        public int[] CountryIds { get; set; } = []; // countries that uses this currency
        public int EmmissionCountryId { get; set; } // country that emits this currency
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
    }
}