using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OctopusAPI.Database.Entities.Trading
{
    public class Item
    {
        [Key]
        public Guid ItemBlueprintId { get; set; }
        [ForeignKey("ItemBlueprintId")]
        public required ItemBlueprint ItemBlueprint { get; set; }
        [Required]
        public required string OwnerId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime OwnedAt { get; set; }
        public decimal Amount { get; set; }
    }
}