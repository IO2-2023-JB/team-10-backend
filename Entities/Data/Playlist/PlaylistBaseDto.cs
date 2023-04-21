using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Playlist
{
	public class PlaylistBaseDto : CreatePlaylistResponseDto
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[EnumDataType(typeof(Visibility))]
		public Visibility Visibility { get; set; }
	}
}
