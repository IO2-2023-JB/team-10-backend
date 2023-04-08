using Entities.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Models
{
	public class Reaction : MongoDocumentBase
	{
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string VideoId { get; set; }

		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string UserId { get; set; }

		public ReactionType ReactionType { get; set; }
	}
}
