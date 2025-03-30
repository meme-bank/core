using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OctopusAPI.Database.Entities.Core;

namespace OctopusAPI.Database.Entities.Trading
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public Photo? Icon { get; set; }
        public List<Service> Services { get; set; } = new List<Service>();
        public List<ItemBlueprint> ItemBlueprints { get; set; } = new List<ItemBlueprint>();
    }
}