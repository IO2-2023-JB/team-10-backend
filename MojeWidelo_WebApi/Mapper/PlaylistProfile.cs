using Entities.Data.Playlist;
using Entities.Models;

namespace MojeWidelo_WebApi.Mapper
{
	public class PlaylistProfile : AutoMapper.Profile
	{
		public PlaylistProfile()
		{
			CreateMap<CreatePlaylistRequestDto, Playlist>();
			CreateMap<Playlist, CreatePlaylistResponseDto>();
			CreateMap<Playlist, PlaylistDto>();
			CreateMap<Playlist, PlaylistBaseDto>();
		}
	}
}
