using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Playlist
{
    public abstract class PlaylistBaseDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Count { get; set; }
        [Required]
        [EnumDataType(typeof(Visibility))]
        public Visibility Visibility { get; set; } 
    }
}
