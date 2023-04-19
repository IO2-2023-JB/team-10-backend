using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Data.Comment
{
	public class CommentDto
	{
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string Id { get; set; }

		public string AuthorId { get; set; }

		public string Content { get; set; }

		public string AvatarImage { get; set; }

		public string Nickname { get; set; }

		public bool hasResponses { get; set; }
	}
}
