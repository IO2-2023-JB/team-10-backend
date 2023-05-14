using Entities.Data.Playlist;
using Entities.Data.User;
using Entities.Data.Video;

namespace Entities.Data.Search
{
	public class SearchResultDto
	{
		public IEnumerable<UserDto> Users { get; set; }
		public IEnumerable<VideoMetadataDto> Videos { get; set; }
		public IEnumerable<PlaylistBaseDto> Playlists { get; set; }
	}
}
