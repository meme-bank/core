using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OctopusAPI.Database.Entities.Economic
{
    public class TransferNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public required Wallet Sender { get; set; }
        public required string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public required Wallet Receiver { get; set; }
        public decimal Amount { get; set; }
        public required Currency Currency { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime NotedAt { get; set; }
        public string? Description { get; set; }
    }
}