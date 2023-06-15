using System.Text.Json.Serialization;

namespace Entities.Data.Playlist
{
	public class RecommendationDto
	{
		[JsonPropertyName("video_id")]
		public string VideoId { get; set; }
	}
}
