using AutoMapper;
using Contracts;
using Entities.Data.Playlist;
using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using Repository.Managers;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text.Json;

namespace MojeWidelo_WebApi.Controllers
{
	[ApiController]
	public class PlaylistController : BaseController
	{
		private readonly VideoManager _videoManager;

		public PlaylistController(IRepositoryWrapper repository, IMapper mapper, VideoManager videoManager)
			: base(repository, mapper)
		{
			_videoManager = videoManager;
		}

		/// <summary>
		/// Playlist creation
		/// </summary>
		/// <param name="createPlaylistRequestDto"></param>
		/// <returns></returns>
		/// <response code="201">Created</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		[HttpPost("playlist/details")]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(CreatePlaylistResponseDto))]
		public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequestDto createPlaylistRequestDto)
		{
			var userID = GetUserIdFromToken();
			var playlist = _mapper.Map<Playlist>(createPlaylistRequestDto);
			playlist.AuthorId = userID;
			playlist.Videos = new List<string>();
			playlist.CreationDate = playlist.EditDate = DateTime.Now;

			playlist = await _repository.PlaylistRepository.Create(playlist);
			var result = _mapper.Map<CreatePlaylistResponseDto>(playlist);

			return StatusCode(StatusCodes.Status201Created, result);
		}

		/// <summary>
		/// Edit playlist properties
		/// </summary>
		/// <param name="id"></param>
		/// <param name="playlistEditDto"></param>
		/// <returns></returns>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		[HttpPut("playlist/details")]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(PlaylistDto))]
		public async Task<IActionResult> EditPlaylist([Required] string id, [FromBody] PlaylistEditDto playlistEditDto)
		{
			var playlist = await _repository.PlaylistRepository.GetById(id);

			if (playlist == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Playlista o podanym ID nie istnieje.");
			}

			var userID = GetUserIdFromToken();
			if (userID != playlist.AuthorId)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do edycji playlisty.");
			}

			playlist = _mapper.Map<PlaylistEditDto, Playlist>(playlistEditDto, playlist);

			playlist.EditDate = DateTime.Now;
			playlist = await _repository.PlaylistRepository.Update(id, playlist);

			var result = _mapper.Map<PlaylistDto>(playlist);
			var videos = await _repository.VideoRepository.GetVideos(playlist.Videos, userID);
			result.Videos = _mapper.Map<IEnumerable<VideoMetadataDto>>(videos).ToArray();
			_videoManager.AddThumbnailUri(new Uri($"{Request.Scheme}://{Request.Host}"), result.Videos);
			return StatusCode(StatusCodes.Status200OK, result);
		}

		/// <summary>
		/// Delete playlist
		/// </summary>
		/// <param name="id"></param>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		[HttpDelete("playlist/details")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> DeletePlaylist([Required] string id)
		{
			var playlist = await _repository.PlaylistRepository.GetById(id);

			if (playlist == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Playlista o podanym ID nie istnieje.");
			}

			if (GetUserIdFromToken() != playlist.AuthorId)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do usunięcia playlisty.");
			}

			await _repository.PlaylistRepository.Delete(id);
			return StatusCode(StatusCodes.Status200OK, "Playlista została usunięta pomyślnie.");
		}

		/// <summary>
		/// Get all playlists for user
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="404">Not found</response>
		[HttpGet("playlist/user")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(IEnumerable<PlaylistBaseDto>))]
		public async Task<IActionResult> GetPlaylistsForUser([Required] string id)
		{
			var userID = GetUserIdFromToken();

			var targetUser = await _repository.UsersRepository.GetById(id);
			if (targetUser == null)
				return StatusCode(StatusCodes.Status404NotFound, "Użytkownik o podanym ID nie istnieje.");

			var playlists = await _repository.PlaylistRepository.GetPlaylistByUserId(targetUser.Id, userID);
			var playlistBases = new List<PlaylistBaseDto>();
			foreach (var playlist in playlists)
				playlistBases.Add(_mapper.Map<PlaylistBaseDto>(playlist));
			return StatusCode(StatusCodes.Status200OK, playlistBases.ToArray());
		}

		/// <summary>
		/// Get all videos in playlist
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		[HttpGet("playlist/video")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(PlaylistDto))]
		public async Task<IActionResult> GetVideosInPlaylist([Required] string id)
		{
			var playlist = await _repository.PlaylistRepository.GetById(id);

			if (playlist == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Playlista o podanym ID nie istnieje.");
			}

			var userID = GetUserIdFromToken();
			if (playlist.Visibility == PlaylistVisibility.Private && userID != playlist.AuthorId)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do dostępu do playlisty.");
			}

			var result = _mapper.Map<PlaylistDto>(playlist);
			var author = await _repository.UsersRepository.GetById(result.AuthorId);
			result.AuthorNickname = author.Nickname;

			var videos = await _repository.VideoRepository.GetVideos(playlist.Videos, userID);
			result.Videos = _mapper.Map<IEnumerable<VideoMetadataDto>>(videos).ToArray();
			var users = (await _repository.UsersRepository.GetUsersByIds(result.Videos.Select(x => x.AuthorId)));
			_videoManager.AddAuthorNickname(result.Videos, users);
			_videoManager.AddThumbnailUri(new Uri($"{Request.Scheme}://{Request.Host}"), result.Videos);

			return StatusCode(StatusCodes.Status200OK, result);
		}

		/// <summary>
		/// Add video to playlist
		/// </summary>
		/// <returns></returns>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		[HttpPost("playlist/{id}/{videoId}")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> AddVideoToPlaylist(string id, string videoId)
		{
			var playlist = await _repository.PlaylistRepository.GetById(id);

			if (playlist == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Playlista o podanym ID nie istnieje.");
			}

			if (GetUserIdFromToken() != playlist.AuthorId)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do edycji playlisty.");
			}

			var video = await _repository.VideoRepository.GetById(videoId);

			if (video == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Wideo o podanym ID nie istnieje.");
			}

			var videos = playlist.Videos.ToList();
			if (videos.Contains(videoId))
			{
				return StatusCode(
					StatusCodes.Status400BadRequest,
					"Wideo o podanym ID znajduje się już w tej playliście."
				);
			}
			playlist.Videos = playlist.Videos.Append(videoId);
			playlist.EditDate = DateTime.Now;
			await _repository.PlaylistRepository.Update(id, playlist);
			return StatusCode(StatusCodes.Status200OK, "Wideo zostało dodane do playlisty.");
		}

		/// <summary>
		/// Remove video from playlist
		/// </summary>
		/// <returns></returns>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		[HttpDelete("playlist/{id}/{videoId}")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> RemoveVideoFromPlaylist(string id, string videoId)
		{
			var playlist = await _repository.PlaylistRepository.GetById(id);

			if (playlist == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Playlista o podanym ID nie istnieje.");
			}

			if (GetUserIdFromToken() != playlist.AuthorId)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do edycji playlisty.");
			}

			var videos = playlist.Videos.ToList();
			if (!videos.Contains(videoId))
			{
				return StatusCode(
					StatusCodes.Status400BadRequest,
					"Wideo o podanym ID nie znajduje się w tej playliście."
				);
			}
			playlist.Videos = playlist.Videos.Where(x => x != videoId);
			playlist.EditDate = DateTime.Now;
			await _repository.PlaylistRepository.Update(id, playlist);
			return StatusCode(StatusCodes.Status200OK, "Wideo zostało usunięte z playlisty.");
		}

		/// <summary>
		/// Get recommended videos for current user
		/// </summary>
		/// <returns></returns>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		[HttpGet("playlist/recommended")]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(PlaylistDto))]
		public async Task<IActionResult> GetRecommendedVideos()
		{
			var user = await GetUserFromToken();

			var videoIDs = await _videoManager.GetRecommendationsFromEngine(user.Id);
			var videos = await _repository.VideoRepository.GetMoreVideosToRecommend(
				videoIDs,
				user.Id,
				await _repository.SubscriptionsRepository.GetUserSubscriptions(user.Id)
			);
			var result = _mapper.Map<IEnumerable<VideoMetadataDto>>(videos);
			var users = (await _repository.UsersRepository.GetUsersByIds(result.Select(x => x.AuthorId)));
			_videoManager.AddAuthorNickname(result, users);
			_videoManager.AddThumbnailUri(new Uri($"{Request.Scheme}://{Request.Host}"), result);

			var recommended = new PlaylistDto();
			recommended.Name = "Video recommendation " + DateTime.Now.ToString();
			recommended.Visibility = PlaylistVisibility.Private;
			recommended.Videos = result;
			recommended.AuthorId = user.Id;
			recommended.AuthorNickname = user.Nickname;

			return StatusCode(StatusCodes.Status200OK, recommended);
		}
	}
}
