using Entities.Data.Video;

namespace Entities.Data.Playlist
{
	public class PlaylistDto : CreatePlaylistRequestDto
	{
		public IEnumerable<VideoMetadataDto> Videos { get; set; }
	}
}
