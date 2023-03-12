using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace Entities.Models
{
    public class User : MongoDocumentBase
    {
        [BsonElement(nameof(Email))]
        public string Email
        {
            get; set;
        }

        [BsonElement(nameof(Nickname))]
        public string Nickname
        {
            get; set;
        }

        [BsonElement(nameof(Name))]
        public string Name
        {
            get; set;
        }

        [BsonElement(nameof(Surname))]
        public string Surname
        {
            get; set;
        }

        [BsonElement(nameof(AccountBalance))]
        public double AccountBalance
        {
            get; set;
        }

#warning userType enum czy co
    }
}
