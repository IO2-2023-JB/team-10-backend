using AutoMapper;
using Contracts;
using Entities.Data;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using System.Net.Mime;

namespace MojeWidelo_WebApi.Controllers
{
    [Route("api/")]
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
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        [HttpPost("video")]
        [ServiceFilter(typeof(ModelValidationFilter))]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(VideoMetadataDTO))]
        public async Task<IActionResult> UploadVideoMetadata([FromBody] VideoUploadDTO videoUploadDto)
        {
#warning dodać autora, logikę ProcessingProgress, itp.
            var video = _mapper.Map<VideoMetadata>(videoUploadDto);
            video.UploadDate = video.EditDate = DateTime.Now;
            video = await _repository.VideoRepository.Create(video);
            var result = _mapper.Map<VideoMetadataDTO>(video);
            return Ok(result);
        }
    }
}
