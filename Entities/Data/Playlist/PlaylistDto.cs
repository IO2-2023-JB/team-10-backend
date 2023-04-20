using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Playlist
{
    public class PlaylistDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EnumDataType(typeof(Visibility))]
        public Visibility Visibility { get; set; }
        [Required]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public IEnumerable<string> Videos { get; set; }
    }
}
