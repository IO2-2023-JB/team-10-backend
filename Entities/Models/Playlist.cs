using Entities.Data.Video;
using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
	public class Playlist : MongoDocumentBase
	{
		public string Name { get; set; }

		[EnumDataType(typeof(Visibility))]
		public Visibility Visibility { get; set; }

		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string AuthorId { get; set; }

		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public IEnumerable<string> Videos { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime EditDate { get; set; }
	}
}
