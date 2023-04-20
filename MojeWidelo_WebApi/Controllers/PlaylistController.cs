using AutoMapper;
using Contracts;
using Entities.Data.Playlist;
using Entities.Data.Video;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using Repository.Managers;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace MojeWidelo_WebApi.Controllers
{
    [ApiController]
    public class PlaylistController : BaseController
    {
        private readonly PlaylistManager _playlistManager;

        public PlaylistController(IRepositoryWrapper repository, IMapper mapper, PlaylistManager manager)
            : base(repository, mapper)
        {
            _playlistManager = manager;
        }

        /// <summary>
        /// Playlist creation
        /// </summary>
        /// <param name="createPlaylistRequestDto"></param>
        /// <returns></returns>
        /// <response code="201">Created</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        [HttpPost("playlist")]
        [ServiceFilter(typeof(ModelValidationFilter))]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(CreatePlaylistResponseDto))]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequestDto createPlaylistRequestDto)
        {
            var user = await GetUserFromToken();
            var playlist = _mapper.Map<Playlist>(createPlaylistRequestDto);
            playlist.AuthorId = user.Id;
            playlist.Videos = new List<string>();
            playlist.CreationDate = DateTime.Now;
            playlist.EditDate = DateTime.Now;

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
        [HttpPut("playlist")]
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

            if (GetUserIdFromToken() != playlist.AuthorId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do edycji playlisty.");
            }

            playlist = _mapper.Map<PlaylistEditDto, Playlist>(playlistEditDto, playlist);

            playlist.EditDate = DateTime.Now;
            playlist = await _repository.PlaylistRepository.Update(id, playlist);
            var result = _mapper.Map<PlaylistDto>(playlist);
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
        [HttpDelete("playlist")]
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
                return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do edycji playlisty.");
            }

            await _repository.PlaylistRepository.Delete(id);
            return StatusCode(StatusCodes.Status200OK, "Użytkownik został usunięty pomyślnie.");
        }

        /// <summary>
        /// Get all playlists for user
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        [HttpGet("playlist/user")]
        public async Task<IActionResult> GetPlaylistsForUser()
        {


        }
    }
}
