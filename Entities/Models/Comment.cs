using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Entities.Models
{
    public class Comment : MongoDocumentBase
    {
        public Comment(string videoId, string authorId, string content, string avatarImage, string nickname, bool hasResponses = false)
        {
            VideoId = videoId;
            AuthorId = authorId;
            Content = content;
            AvatarImage = avatarImage;
            Nickname = nickname;
            this.hasResponses = hasResponses;
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string VideoId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; }

        public string Content { get; set; }

        public string AvatarImage { get; set; }

        public string Nickname { get; set; }

        public bool hasResponses { get; set; }
    }
}
