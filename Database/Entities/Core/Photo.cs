using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OctopusAPI.Database.Entities.Core
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public byte[]? Image { get; set; }
        public PhotoType Type { get; set; } = PhotoType.Unknown;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime RequestedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UploadedAt { get; set; }
    }

    public enum PhotoType
    {
        Item, // 512x512 (1/1)
        Avatar, // 128x128 (1/1)
        Cover, // 1280x512 (5/2)
        Icon, // 64x64 (1/1)
        Unknown // Any other type (maximum 1280x1280)
    }
}