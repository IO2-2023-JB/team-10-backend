using AutoMapper;
using Contracts;
using Entities.Data;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using System.Net.Mime;

namespace MojeWidelo_WebApi.Controllers
{
    [ApiController]
    public class VideoController : BaseController
    {
        public VideoController(IRepositoryWrapper repository, IMapper mapper) : base(repository, mapper)
        {
        }

        /// <summary>
        /// Video metadata upload
        /// </summary>
        /// <param name="videoUploadDto"></param>
        /// <returns></returns>
        /// <response code="201">Created</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        [HttpPost("video")]
        [ServiceFilter(typeof(ModelValidationFilter))]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(VideoMetadataDTO))]
        public async Task<IActionResult> UploadVideoMetadata([FromBody] VideoUploadDTO videoUploadDto)
        {
            var user = await GetUserFromToken();

#warning dodać logikę ProcessingProgress

            // mapowanie i uzupełnienie danych
            var video = _mapper.Map<VideoMetadata>(videoUploadDto);
            video.UploadDate = video.EditDate = DateTime.Now;
            video.AuthorId = user.Id;
            video.AuthorNickname = user.Nickname;

            video = await _repository.VideoRepository.Create(video);
            var result = _mapper.Map<VideoMetadataDTO>(video);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// Video metadata update
        /// </summary>
        /// <param name="videoUpdateDTO"></param>
        /// <returns></returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="404">Not found</response>
        [HttpPut("video-metadata")]
        [ServiceFilter(typeof(ModelValidationFilter))]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(VideoMetadataDTO))]
        public async Task<IActionResult> UpdateVideoMetadata([FromBody] VideoUpdateDTO videoUpdateDTO)
        {
            // pobieram stare dane, potem mapuje te pola które nie są nullami + omijam te immutable (patrz MappingProfile.cs)
            var video = await _repository.VideoRepository.GetById(videoUpdateDTO.Id);

            if (video == null)
            {
                return NotFound();
            }

            video = _mapper.Map<VideoUpdateDTO, VideoMetadata>(videoUpdateDTO, video);

            video.EditDate = DateTime.Now;
            video = await _repository.VideoRepository.Update(videoUpdateDTO.Id, video);
            var result = _mapper.Map<VideoMetadataDTO>(video);
            return Ok(result);
        }
    }
}
