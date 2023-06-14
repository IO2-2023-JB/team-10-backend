using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
	public class Ticket : MongoDocumentBase
	{
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string SubmitterId { get; set; }

		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string TargetId { get; set; }

		[EnumDataType(typeof(TicketTargetTypeDto))]
		public TicketTargetTypeDto TargetType { get; set; }

		public string Reason { get; set; }

		[EnumDataType(typeof(TicketStatus))]
		public TicketStatus Status { get; set; }

		public string? Response { get; set; }

		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string? AdminId { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime? ResponseDate { get; set; }
	}
}
