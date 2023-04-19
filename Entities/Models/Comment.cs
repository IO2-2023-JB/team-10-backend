using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Entities.Models
{
	public class Comment : MongoDocumentBase
	{
		[BsonRepresentation(BsonType.ObjectId)]
		public string VideoId { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string AuthorId { get; set; }

		public string Content { get; set; }

		public bool HasResponses { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime LastModificationDate { get; set; }

		public Comment(string videoId, string authorId, string content, bool hasResponses = false)
		{
			VideoId = videoId;
			AuthorId = authorId;
			Content = content;
			HasResponses = hasResponses;
			CreationDate = DateTime.Now;
			LastModificationDate = DateTime.Now;
		}
	}
}
