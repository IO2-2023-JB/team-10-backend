using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Models
{
	public class Subscription : MongoDocumentBase
	{
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string CreatorId { get; set; }

		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string SubscriberId { get; set; }
	}
}
