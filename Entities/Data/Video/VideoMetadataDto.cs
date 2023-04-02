using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Video
{
	public class VideoMetadataDto : VideoUpdateDto
	{
		[BsonId]
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string AuthorId { get; set; }

		public string AuthorNickname { get; set; }

		public int ViewCount { get; set; }

		[EnumDataType(typeof(ProcessingProgress))]
		public ProcessingProgress ProcessingProgress { get; set; }

		public DateTime UploadDate { get; set; }

		public DateTime EditDate { get; set; }

		public string Duration { get; set; }
	}
}
