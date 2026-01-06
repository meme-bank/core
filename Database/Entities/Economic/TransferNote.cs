using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OctopusAPI.Database.Entities.Economic
{
    public class TransferNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public TransferNoteType Type { get; set; }
        public required Guid SenderId { get; set; }
        [ForeignKey("SenderId")]
        public required Wallet Sender { get; set; }
        public required Guid ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public required Wallet Receiver { get; set; }
        public decimal Amount { get; set; }
        [ForeignKey("CurrencyId")]
        public required Currency Currency { get; set; }
        public required string CurrencyId { get; set; }
        public decimal ExchangeRateAtTransfer { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime NotedAt { get; set; }
        public string? Description { get; set; }
    }
    public enum TransferNoteType
    {
        Personal,
        Buying,
        Tax,
        Emission,
        Subscription
    }
}