﻿using Entities.Enums;
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

		[BsonRepresentation(BsonType.Decimal128)]
		public decimal AccountBalance { get; set; }

		public UserType UserType { get; set; }

		public string Password { get; set; }

		public int SubscriptionsCount { get; set; }

		public string AvatarImage { get; set; }

		public override bool Equals(object? obj)
		{
			return obj is User user && Id == user.Id;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id);
		}
	}
}
