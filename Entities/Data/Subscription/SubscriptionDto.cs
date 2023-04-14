using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Subscription
{
	public class SubscriptionDto
	{
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string Id { get; set; }

		public string? AvatarImage { get; set; }

		[Required]
		public string Nickname { get; set; }
	}
}
