using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Video
{
	public class VideoUploadResponseDto
	{
		[BsonId]
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string Id { get; set; }

		[EnumDataType(typeof(ProcessingProgress))]
		public ProcessingProgress ProcessingProgress { get; set; }
	}
}
