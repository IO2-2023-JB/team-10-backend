using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
	public class Ticket : MongoDocumentBase
	{
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string TargetId { get; set; }

		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string AuthorId { get; set; }

		public string Reason { get; set; }

		[EnumDataType(typeof(TicketStatus))]
		public TicketStatus Status { get; set; }

		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string? AdminId { get; set; }

		public string? Response { get; set; }
	}
}
