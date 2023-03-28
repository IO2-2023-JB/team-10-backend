using Entities.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Models
{
	public class User : MongoDocumentBase
	{
		public string Email { get; set; }

		public string Nickname { get; set; }

		public string Name { get; set; }

		public string Surname { get; set; }

		public double AccountBalance { get; set; }

		public UserType UserType { get; set; }

		public string Password { get; set; }

		public int SubscriptionsCount { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AvatarImage
        {
            get; set;
        }
    }
}
