using AutoMapper;
using Contracts;
using Entities.Data.User;
using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using System.Net.Mime;

namespace MojeWidelo_WebApi.Controllers
{
	[ApiController]
	public class VideoController : BaseController
	{
		public VideoController(IRepositoryWrapper repository, IMapper mapper)
			: base(repository, mapper) { }

		/// <summary>
		/// Video metadata upload
		/// </summary>
		/// <param name="videoUploadDto"></param>
		/// <returns></returns>
		/// <response code="201">Created</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		[HttpPost("video-metadata")]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(VideoMetadataDto))]
		public async Task<IActionResult> UploadVideoMetadata([FromBody] VideoUploadDto videoUploadDto)
		{
			var user = await GetUserFromToken();
			if (user.UserType != UserType.Creator)
				return StatusCode(
					StatusCodes.Status403Forbidden,
					"Brak uprawnień do przesyłania filmów. Użytkownik nie jest twórcą."
				);

			// mapowanie i uzupełnienie danych
			var video = _mapper.Map<VideoMetadata>(videoUploadDto);
			video.UploadDate = video.EditDate = DateTime.Now;
			video.AuthorId = user.Id;
			video.AuthorNickname = user.Nickname;
			video.ProcessingProgress = ProcessingProgress.MetadataRecordCreated;

			video = await _repository.VideoRepository.Create(video);
			var result = _mapper.Map<VideoMetadataDto>(video);
			return StatusCode(StatusCodes.Status201Created, result);
		}

		/// <summary>
		/// Video metadata update
		/// </summary>
		/// <param name="id"></param>
		/// <param name="videoUpdateDto"></param>
		/// <returns></returns>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		[HttpPut("video-metadata/{id}")]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(VideoMetadataDto))]
		public async Task<IActionResult> UpdateVideoMetadata(string id, [FromBody] VideoUpdateDto videoUpdateDto)
		{
			// pobieram stare dane, omijam te immutable (patrz MappingProfile.cs)
			var video = await _repository.VideoRepository.GetById(id);

			if (video == null)
			{
				return NotFound();
			}

			if (GetUserIdFromToken() != video.AuthorId)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "No permissions to edit video metadata");
			}

			video = _mapper.Map<VideoUpdateDto, VideoMetadata>(videoUpdateDto, video);

			video.EditDate = DateTime.Now;
			video = await _repository.VideoRepository.Update(id, video);
			var result = _mapper.Map<VideoMetadataDto>(video);
			return Ok(result);
		}

		/// <summary>
		/// Video metadata retrieval
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		[HttpGet("video-metadata/{id}")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(VideoMetadataDto))]
		public async Task<IActionResult> GetVideoMetadataById(string id)
		{
			var video = await _repository.VideoRepository.GetById(id);

			if (video == null)
				return NotFound();

			if (video.Visibility == VideoVisibility.Private && GetUserIdFromToken() != video.AuthorId)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "No permissions to get video metadata");
			}

			var result = _mapper.Map<VideoMetadataDto>(video);
			return Ok(result);
		}

		/// <summary>
		/// Video upload
		/// </summary>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		/// <response code="500">Internal server error</response>
		/// <response code="501">Not implemented</response>
		[HttpPost("video/{id}", Name = "uploadVideo")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		[ServiceFilter(typeof(VideoExtensionValidationFilter))]
		[DisableRequestSizeLimit]
		[RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = long.MaxValue)]
		public async Task<IActionResult> UploadVideo(string id, [FromForm] IFormFile videoFile)
		{
			var video = await _repository.VideoRepository.GetById(id);

			if (video == null)
				return NotFound("No video under provided ID");

			if (GetUserIdFromToken() != video.AuthorId)
				return StatusCode(StatusCodes.Status403Forbidden);

			if (
				video.ProcessingProgress != ProcessingProgress.MetadataRecordCreated
				&& video.ProcessingProgress != ProcessingProgress.FailedToUpload
			)
				return BadRequest("Video is in state that doesn't allow for upload");

			string? path = _repository.VideoRepository.CreateNewPath(id, videoFile.FileName);
			if (path == null)
				return StatusCode(StatusCodes.Status501NotImplemented);

			try
			{
				_repository.VideoRepository.ChangeVideoProcessingProgress(id, ProcessingProgress.Uploading);

				using var stream = new FileStream(path, FileMode.Create);
				await videoFile.CopyToAsync(stream);
			}
			catch (Exception)
			{
				_repository.VideoRepository.ChangeVideoProcessingProgress(id, ProcessingProgress.FailedToUpload);

				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			_repository.VideoRepository.ChangeVideoProcessingProgress(id, ProcessingProgress.Uploaded);

			// PROCESSING PATCH

			if (!System.IO.File.Exists(path))
				return StatusCode(StatusCodes.Status500InternalServerError);
			System.IO.File.Move(path, _repository.VideoRepository.GetReadyFilePath(id));

			_repository.VideoRepository.ChangeVideoProcessingProgress(id, ProcessingProgress.Ready);

			// END OF PROCESSING PATCH

			return Ok("Upload completed successfully");
		}

		/// <summary>
		/// Video straming
		/// </summary>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		/// <response code="500">Internal server error</response>
		/// <response code="501">Not implemented</response>
		[HttpGet("video/{id}", Name = "streamVideo")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> StreamVideo(string id)
		{
			var video = await _repository.VideoRepository.GetById(id);

			if (video == null)
				return NotFound("No video under provided ID");

			if (video.Visibility == VideoVisibility.Private && GetUserIdFromToken() != video.AuthorId)
				return StatusCode(StatusCodes.Status403Forbidden);

			if (video.ProcessingProgress != ProcessingProgress.Ready)
				return BadRequest("Video is in state that doesn't allow for streaming");

			string path = _repository.VideoRepository.GetReadyFilePath(id);
			if (path == null)
				return StatusCode(StatusCodes.Status501NotImplemented);

			if (!System.IO.File.Exists(path))
				return StatusCode(StatusCodes.Status500InternalServerError);

			var res = File(System.IO.File.OpenRead(path), "video/mp4", true);
			return res;
		}

        /// <summary>
        /// *Endpoint for testing*
        /// </summary>
        /// <returns>List of all videos</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("getAllVideos", Name = "GetAllVideos")]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(IEnumerable<VideoMetadataDto>))]
        public async Task<IActionResult> GetAllVideos()
        {
            var videos = await _repository.VideoRepository.GetAll();
            var result = _mapper.Map<IEnumerable<VideoMetadataDto>>(videos);
            return Ok(result);
        }
    }
}
