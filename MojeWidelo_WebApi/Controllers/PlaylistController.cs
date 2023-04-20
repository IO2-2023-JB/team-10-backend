using AutoMapper;
using Contracts;
using Entities.Data.Playlist;
using Entities.Data.Video;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using Repository.Managers;
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
        /// <param name="CreatePlaylistRequestDto"></param>
        /// <returns></returns>
        /// <response code="201">Created</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        [HttpPost("video-metadata")]
        [ServiceFilter(typeof(ModelValidationFilter))]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(CreatePlaylistResponseDto))]
        public async Task<IActionResult> createPlaylist([FromBody] CreatePlaylistRequestDto CreatePlaylistRequestDto)
        {
            var user = await GetUserFromToken();
            var playlist = _mapper.Map<Playlist>(CreatePlaylistRequestDto);
            playlist.AuthorId = user.Id;
            playlist.Videos = new List<VideoBaseDto>();
            playlist.CreationDate = DateTime.Now;
            playlist.EditDate = DateTime.Now;

            playlist = await _repository.PlaylistRepository.Create(playlist);
            var result = _mapper.Map<CreatePlaylistResponseDto>(playlist);

            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}
