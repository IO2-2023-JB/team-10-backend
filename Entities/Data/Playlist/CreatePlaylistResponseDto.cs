using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Data.Playlist
{
    public class CreatePlaylistResponseDto
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
