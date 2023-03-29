using Entities.Enums;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class VideoMetadata : MongoDocumentBase
    {
        public string Title
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public IFormFile? Thumbnail
        {
            get; set;
        }

        public IEnumerable<string> Tags
        {
            get; set;
        }

        [EnumDataType(typeof(VideoVisibility))]
        public VideoVisibility Visibility
        {
            get; set;
        }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string AuthorId
        {
            get; set;
        }

        public string AuthorNickname
        {
            get; set;
        }

        public int ViewCount
        {
            get; set;
        }

        [EnumDataType(typeof(ProcessingProgress))]
        public ProcessingProgress ProcessingProgress
        {
            get; set;
        }

        public DateTime UploadDate
        {
            get; set;
        }

        public DateTime EditDate
        {
            get; set;
        }

        public string Duration
        {
            get; set;
        }
    }
}
