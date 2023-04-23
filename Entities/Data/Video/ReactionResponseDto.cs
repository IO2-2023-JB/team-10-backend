using Entities.Enums;

namespace Entities.Data.Video
{
	public class ReactionResponseDto
	{
		public int PositiveCount { get; set; }

		public int NegativeCount { get; set; }

		public ReactionType CurrentUserReaction { get; set; }
	}
}
