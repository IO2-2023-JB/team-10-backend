using Entities.Data.Video;
using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Playlist : MongoDocumentBase
    {
        public string Name { get; set; }

        public string Count { get; set; }

        [EnumDataType(typeof(Visibility))]
        public Visibility Visibility { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string AuthorId { get; set; }

        public IEnumerable<VideoBaseDto> Videos { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime EditDate { get; set; }
    }
}
