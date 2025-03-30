using OctopusAPI.Database.Entities.Economic;
using OctopusAPI.Database.Entities.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OctopusAPI.Database.Entities.Trading
{
    public class Service
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public required Guid PhotoId { get; set; }
        [ForeignKey("PhotoId")]
        public required Photo Photo { get; set; }
        public ServiceType Type { get; set; } = ServiceType.Once;
        public int ProviderId { get; set; }
        public decimal Price { get; set; }
        [Required]
        public required string CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public required Currency Currency { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public TimeSpan? Duration { get; set; } // Nullable for once services or subscription services with no duration or end date
        [Required]
        public DateTime PublishedAt { get; set; }

        public ICollection<ProvideService> ProvideServices { get; set; } = new List<ProvideService>();
    }

    public enum ServiceType
    {
        Once,
        Subscription,
    }
}