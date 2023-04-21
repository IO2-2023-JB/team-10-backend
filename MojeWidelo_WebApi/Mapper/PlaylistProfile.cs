using Entities.Data.Playlist;
using Entities.Data.Video;
using Entities.Models;

namespace MojeWidelo_WebApi.Mapper
{
	public class PlaylistProfile : AutoMapper.Profile
	{
		public PlaylistProfile()
		{
			CreateMap<CreatePlaylistRequestDto, Playlist>();
			CreateMap<Playlist, CreatePlaylistResponseDto>();
			CreateMap<Playlist, PlaylistDto>().ForMember(playlist => playlist.Videos, opt => opt.Ignore());
			CreateMap<Playlist, PlaylistBaseDto>();
		}
	}
}
