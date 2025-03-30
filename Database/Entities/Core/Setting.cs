using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OctopusAPI.Database.Entities.Core
{
    public class Setting
    {
        [Key]
        public required string Key { get; set; }
        public string? DisplayKey { get; set; }
        [Required]
        public required string Value { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ChangedAt { get; set; }
        public int? ChangedAtById { get; set; }
    }
}