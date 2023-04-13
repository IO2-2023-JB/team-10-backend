using AutoMapper;
using Contracts;
using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using Repository.Managers;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace MojeWidelo_WebApi.Controllers
{
	[ApiController]
	public class VideoController : BaseController
	{
		private readonly VideoManager _videoManager;

		public VideoController(IRepositoryWrapper repository, IMapper mapper, VideoManager videoManager)
			: base(repository, mapper)
		{
			_videoManager = videoManager;
		}

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
		[HttpPut("video-metadata")]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(VideoMetadataDto))]
		public async Task<IActionResult> UpdateVideoMetadata(
			[Required] string id,
			[FromBody] VideoUpdateDto videoUpdateDto
		)
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
		[HttpGet("video-metadata")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(VideoMetadataDto))]
		public async Task<IActionResult> GetVideoMetadataById([Required] string id)
		{
			var video = await _repository.VideoRepository.GetById(id);

			if (video == null)
				return NotFound();

			if (video.Visibility == VideoVisibility.Private && GetUserIdFromToken() != video.AuthorId)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "No permissions to get video metadata");
			}

			video.ViewCount++;
			video = await _repository.VideoRepository.Update(video.Id, video);

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

			string? path = _videoManager.CreateNewPath(id, videoFile.FileName);
			if (path == null)
				return StatusCode(StatusCodes.Status501NotImplemented);

			try
			{
				await _repository.VideoRepository.ChangeVideoProcessingProgress(id, ProcessingProgress.Uploading);

				using var stream = new FileStream(path, FileMode.Create);
				await videoFile.CopyToAsync(stream);
			}
			catch (Exception)
			{
				await _repository.VideoRepository.ChangeVideoProcessingProgress(id, ProcessingProgress.FailedToUpload);

				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			await _repository.VideoRepository.ChangeVideoProcessingProgress(id, ProcessingProgress.Uploaded);

			Thread t = new Thread(() => _repository.VideoRepository.ProccessVideoFile(id, path));
			t.Start();

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
		public async Task<IActionResult> StreamVideo([Required] string access_token, string id)
		{
			var video = await _repository.VideoRepository.GetById(id);

			if (video == null)
				return NotFound("No video under provided ID");

			if (video.Visibility == VideoVisibility.Private && GetUserIdFromToken() != video.AuthorId)
				return StatusCode(StatusCodes.Status403Forbidden);

			if (video.ProcessingProgress != ProcessingProgress.Ready)
				return BadRequest("Video is in state that doesn't allow for streaming");

			string? path = _videoManager.GetReadyFilePath(id);
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

		[HttpDelete("video", Name = "deleteVideo")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> DeleteVideo([Required] string id)
		{
			var userId = GetUserIdFromToken();
			var video = await _repository.VideoRepository.GetById(id);

			if (video == null)
			{
				return NotFound("Nie znaleziono wideo o podanym ID.");
			}

			// dodać logikę że jak jest administratorem to może nawet jak nie jego wideo??
			if (video.AuthorId != userId)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Nie jesteś autorem filmu.");
			}

			if (video.ProcessingProgress == ProcessingProgress.Uploading)
			{
				return BadRequest("Nie można usunąć wideo będącego w trakcie wysyłania.");
			}
			if (video.ProcessingProgress == ProcessingProgress.Processing)
			{
				return BadRequest("Nie można usunąć wideo będącego w trakcie przetwarzania.");
			}

			string? location = _videoManager.GetStorageDirectory().Result;
			if (location == null)
				return BadRequest(NotFound("Zmienna środowiskowa dla MojeWideloStorage nie jest ustawiona"));

			string[] filesToDelete = Directory.GetFiles(location, id + "*");

			foreach (var file in filesToDelete)
			{
				if (System.IO.File.Exists(file))
				{
					try
					{
						System.IO.File.Delete(file);
					}
					catch (IOException)
					{
						return BadRequest("Wskazany plik jest aktualnie używany.");
					}
				}
			}

			await _repository.VideoRepository.Delete(id);

			return Ok("Wideo usunięte pomyślnie.");
		}

		/// <summary>
		/// Reaction posting
		/// </summary>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorized</response>
		/// <response code="404">Not found</response>
		[HttpPost("video-reaction", Name = "addReaction")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		[ServiceFilter(typeof(ModelValidationFilter))]
		public async Task<IActionResult> AddReaction([Required] string id, [FromBody] ReactionDto dto)
		{
			var userId = GetUserIdFromToken();

			var currentReaction = await _repository.ReactionRepository.GetCurrentUserReaction(id, userId);
			if (currentReaction.reactionType == ReactionType.None && dto.ReactionType != ReactionType.None)
			{
				var reaction = _mapper.Map<ReactionDto, Reaction>(dto);
				(reaction.VideoId, reaction.UserId) = (id, userId);
				await _repository.ReactionRepository.Create(reaction);
			}
			else if (currentReaction.reactionType != ReactionType.None)
			{
				if (dto.ReactionType == ReactionType.None)
					await _repository.ReactionRepository.Delete(currentReaction.id);
				else
				{
					var reaction = await _repository.ReactionRepository.GetById(currentReaction.id);
					reaction.ReactionType = dto.ReactionType;
					await _repository.ReactionRepository.Update(currentReaction.id, reaction);
				}
			}

			return Ok("Reakcja dodana pomyślnie.");
		}

		/// <summary>
		/// Reaction retrieval
		/// </summary>
		/// <returns>Video reaction counts, requesting user reaction</returns>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="404">Not found</response>
		[HttpGet("video-reaction", Name = "getReaction")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> GetReaction([Required] string id)
		{
			var userId = GetUserIdFromToken();

			var result = await _repository.ReactionRepository.GetReactionsCount(id);
			result.CurrentUserReaction = (
				await _repository.ReactionRepository.GetCurrentUserReaction(id, userId)
			).reactionType;

			return Ok(result);
		}
	}
}
