using AutoMapper;
using Contracts;
using Entities.Data.Playlist;
using Entities.Data.Search;
using Entities.Data.User;
using Entities.Data.Video;
using Microsoft.AspNetCore.Mvc;
using Repository.Managers;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace MojeWidelo_WebApi.Controllers
{
	[ApiController]
	public class SearchController : BaseController
	{
		private readonly VideoManager _videoManager;
		private readonly SearchManager _searchManager;

		public SearchController(
			IRepositoryWrapper repository,
			IMapper mapper,
			SearchManager searchManager,
			VideoManager videoManager
		)
			: base(repository, mapper)
		{
			_videoManager = videoManager;
			_searchManager = searchManager;
		}

		[HttpGet("search")]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(SearchResultDto))]
		public async Task<IActionResult> Search(
			[Required] string query,
			[Required] string sortingCriterion,
			[Required] string sortingType,
			DateTime beginDate,
			DateTime endDate
		)
		{
			var users = await _repository.UsersRepository.GetAll();
			var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);

			var videos = await _repository.VideoRepository.GetAll();
			var videosDto = _mapper.Map<IEnumerable<VideoMetadataDto>>(videos);
			_videoManager.AddAuthorNickname(videosDto, users);

			var playlists = await _repository.PlaylistRepository.GetAll();
			var playlistsDto = _mapper.Map<IEnumerable<PlaylistBaseDto>>(playlists);

			var result = new SearchResultDto()
			{
				Users = _searchManager.SearchUsers(usersDto, query),
				Videos = _searchManager.SearchVideos(videosDto, query),
				Playlists = _searchManager.SearchPlaylists(playlistsDto, query),
			};
			return StatusCode(StatusCodes.Status200OK, result);
		}
	}
}
