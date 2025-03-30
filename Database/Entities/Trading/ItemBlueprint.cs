using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OctopusAPI.Database.Entities.Core;

namespace OctopusAPI.Database.Entities.Trading
{
    public class ItemBlueprint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public required string PhotoId { get; set; }
        [ForeignKey("PhotoId")]
        public required Photo Photo { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public int Rarity { get; set; }
        public MeasuredIn MeasuredIn { get; set; } = MeasuredIn.Pieces; // how this item is measured, for example: pieces, weight, volume (if not pieces then it should be a decimal value)

        public int OwnerCopyrightId { get; set; } // id of the user that owned this item blueprint and can add copyrightids
        public int[] CopyrightIds { get; set; } = []; // ids of the users that can use this item blueprint
        public bool OpenRecipe { get; set; } = true; // if true, anyone can use this item blueprint to craft items

        public ICollection<ItemBlueprintRecipe> Recipe { get; } = new List<ItemBlueprintRecipe>();
        public ICollection<ItemBlueprintRecipe> CraftableItems { get; } = new List<ItemBlueprintRecipe>(); // items that can be crafted from this item

        public string? CraftingStationId { get; set; } // id of the crafting station
        public ItemBlueprint? CraftingStation { get; set; }
        public int? CraftingTime { get; set; } // in seconds
        public int? Health { get; set; } = 0; // for crafting stations
        public int? CraftingStationWear { get; set; } // for craft on crafting stations

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
    }

    public class ItemBlueprintRecipe
    {
        public required string RecipeItemId { get; set; }
        public required ItemBlueprint RecipeItem { get; set; }
        public required string CraftItemId { get; set; }
        public required ItemBlueprint CraftItem { get; set; }
        public decimal Amount { get; set; }
    }

    public enum MeasuredIn
    {
        Pieces,
        Weight,
        Volume,
    }
}