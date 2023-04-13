using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Video
{
	public class ReactionResponseDto
	{
		public int PositiveCount { get; set; }

		public int NegativeCount { get; set; }

		public ReactionType CurrentUserReaction { get; set; }
	}
}
