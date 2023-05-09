using Entities.Enums;

namespace Entities.Data.Playlist
{
	public class PlaylistBaseDto : CreatePlaylistResponseDto
	{
		public string Name { get; set; }
		public PlaylistVisibility Visibility { get; set; }
	}
}
