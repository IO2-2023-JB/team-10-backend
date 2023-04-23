using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Playlist
{
	public class CreatePlaylistRequestDto
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[EnumDataType(typeof(PlaylistVisibility))]
		public PlaylistVisibility Visibility { get; set; }
	}
}
