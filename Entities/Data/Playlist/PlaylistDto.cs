using Entities.Data.Video;
using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Playlist
{
	public class PlaylistDto
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[EnumDataType(typeof(Visibility))]
		public Visibility Visibility { get; set; }

		[Required]
		public IEnumerable<VideoBaseDto> Videos { get; set; }
	}
}
