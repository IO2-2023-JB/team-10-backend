using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Ticket
{
	public class GetTicketDto
	{
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string TicketId { get; set; }

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
	}
}
