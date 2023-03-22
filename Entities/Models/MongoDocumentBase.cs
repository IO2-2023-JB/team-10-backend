using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Entities.Models
{
    public abstract class MongoDocumentBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id
        {
            get; set;
        }
    }
}
