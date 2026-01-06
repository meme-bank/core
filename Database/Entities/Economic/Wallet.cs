using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OctopusAPI.Database.Entities.Economic
{
    public class Wallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int OwnerId { get; set; }
        [Required]
        public required string CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public required Currency Currency { get; set; }
        [Precision(18, 4)]
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<TransferNote> RecivedTransferNotes { get; set; } = new List<TransferNote>();
        public ICollection<TransferNote> SentTransferNotes { get; set; } = new List<TransferNote>();
    }
}