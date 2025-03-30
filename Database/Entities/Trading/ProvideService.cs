using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OctopusAPI.Database.Entities.Economic;

namespace OctopusAPI.Database.Entities.Trading
{
    public class ProvideService
    {
        [Key]
        public required string ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public required Service Service { get; set; }
        [Key]
        [Required]
        public int BuyerId { get; set; }
        public decimal Price { get; set; }
        public required Currency Currency { get; set; }
        public ProvideStatus Status { get; set; } = ProvideStatus.Active;
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime StartAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public enum ProvideStatus
    {
        Active,
        Succesess, // Only for once services
        Cancel,
        Expired
    }
}