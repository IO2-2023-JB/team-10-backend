using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Video
{
    public class ReactionDto
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [Required]
        public string VideoId { get; set; }

        [EnumDataType(typeof(ReactionType))]
        [Required]
        public ReactionType ReactionType { get; set; }
    }
}
