using Entities.Data.Interfaces;
using Entities.Enums;

namespace Entities.Data.Playlist
{
	public class PlaylistBaseDto : CreatePlaylistResponseDto, ISearchable
	{
		public string Name { get; set; }
		public PlaylistVisibility Visibility { get; set; }
	}
}
