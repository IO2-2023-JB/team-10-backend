using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Data.Ticket
{
	public class SubmitTicketDto
	{
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string TargetId { get; set; }
		public string Reason { get; set; }
	}
}
