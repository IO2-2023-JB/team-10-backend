﻿using AutoMapper;
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
		private readonly TimeSpan _viewCountWaitingTime = TimeSpan.FromSeconds(30);

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
		[Produces(MediaTypeNames.Application.Json, Type = typeof(VideoUploadResponseDto))]
		public async Task<IActionResult> UploadVideoMetadata([FromBody] VideoUploadDto videoUploadDto)
		{
			var user = await GetUserFromToken();
			if (user.UserType != UserType.Creator)
				return StatusCode(
					StatusCodes.Status403Forbidden,
					"Brak uprawnień do przesyłania wideo. Użytkownik nie jest twórcą."
				);

			// mapowanie i uzupełnienie danych
			var video = _mapper.Map<VideoMetadata>(videoUploadDto);
			await _repository.VideoRepository.SetThumbnail(video, videoUploadDto);
			video.UploadDate = video.EditDate = DateTime.Now;
			video.AuthorId = user.Id;
			video.ProcessingProgress = ProcessingProgress.MetadataRecordCreated;

			video = await _repository.VideoRepository.Create(video);
			var result = _mapper.Map<VideoUploadResponseDto>(video);

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
				return StatusCode(StatusCodes.Status404NotFound, "Wideo o podanym ID nie istnieje.");
			}

			if (GetUserIdFromToken() != video.AuthorId)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do edycji metadanych.");
			}

			video = _mapper.Map<VideoUpdateDto, VideoMetadata>(videoUpdateDto, video);

			await _repository.VideoRepository.SetThumbnail(video, videoUpdateDto);
			video.EditDate = DateTime.Now;
			video = await _repository.VideoRepository.Update(id, video);
			var result = _mapper.Map<VideoMetadataDto>(video);
			return StatusCode(StatusCodes.Status200OK, result);
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
			var user = await GetUserFromToken();
			var video = await _repository.VideoRepository.GetById(id);

			if (video == null)
				return StatusCode(StatusCodes.Status404NotFound, "Wideo o podanym ID nie istnieje.");

			if (
				video.Visibility == VideoVisibility.Private
				&& user.Id != video.AuthorId
				&& user.UserType != UserType.Administrator
			)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do dostępu do metadanych.");
			}

			if (video.ProcessingProgress == ProcessingProgress.Ready)
			{
				var date = await _repository.HistoryRepository.GetDateTimeOfLastWatchedVideoById(user.Id, id);
				if (date == null || DateTime.Now - date > _viewCountWaitingTime)
				{
					video = await _repository.VideoRepository.UpdateViewCount(video.Id, 1);
					await _repository.HistoryRepository.AddToHistory(user.Id, video.Id);
				}
			}

			var result = _mapper.Map<VideoMetadataDto>(video);
			var author = await _repository.UsersRepository.GetById(result.AuthorId);
			result.AuthorNickname = author.Nickname;

			if (result.Thumbnail != null)
			{
				Uri location = new Uri($"{Request.Scheme}://{Request.Host}");
				result.Thumbnail = location.AbsoluteUri + result.Thumbnail;
			}

			return StatusCode(StatusCodes.Status200OK, result);
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
				return StatusCode(StatusCodes.Status404NotFound, "Wideo o podanym ID nie istnieje.");

			if (GetUserIdFromToken() != video.AuthorId)
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do przesłania wideo.");

			if (
				video.ProcessingProgress != ProcessingProgress.MetadataRecordCreated
				&& video.ProcessingProgress != ProcessingProgress.FailedToUpload
				&& video.ProcessingProgress != ProcessingProgress.FailedToProcess
			)
				return StatusCode(
					StatusCodes.Status400BadRequest,
					"Stan wideo nie pozwala na przesłanie go. Obecny stan: " + video.ProcessingProgress.ToString() + '.'
				);

			string? path = _videoManager.CreateNewPath(id, videoFile.FileName);
			if (path == null)
				return StatusCode(
					StatusCodes.Status501NotImplemented,
					"System nie posiada zdefiniowanej lokalizacji przechowania plików wideo."
				);

			try
			{
				await _repository.VideoRepository.ChangeVideoProcessingProgress(id, ProcessingProgress.Uploading);

				using var stream = new FileStream(path, FileMode.Create);
				await videoFile.CopyToAsync(stream);
			}
			catch (Exception)
			{
				await _repository.VideoRepository.ChangeVideoProcessingProgress(id, ProcessingProgress.FailedToUpload);

				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"Wystąpił błąd przesyłania. Przesyłanie przerwane."
				);
			}

			await _repository.VideoRepository.ChangeVideoProcessingProgress(id, ProcessingProgress.Uploaded);

			Thread t = new Thread(() => _repository.VideoRepository.ProcessAndAddDuration(id, path));
			t.Start();

			return StatusCode(StatusCodes.Status200OK, "Przesyłanie zakończone pomyślnie.");
		}

		/// <summary>
		/// Video streaming
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
				return StatusCode(StatusCodes.Status404NotFound, "Wideo o podanym ID nie istnieje.");

			if (video.Visibility == VideoVisibility.Private && GetUserIdFromToken() != video.AuthorId)
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do streamowania wideo.");

			if (video.ProcessingProgress != ProcessingProgress.Ready)
				return StatusCode(
					StatusCodes.Status400BadRequest,
					"Stan wideo nie pozwala na streaming. Obecny stan: " + video.ProcessingProgress.ToString() + '.'
				);

			string? path = _videoManager.GetReadyFilePath(id);
			if (path == null)
				return StatusCode(
					StatusCodes.Status501NotImplemented,
					"System nie posiada zdefiniowanej lokalizacji przechowania plików wideo."
				);

			if (!System.IO.File.Exists(path))
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"Plik wideo nie istnieje, mimo że powinien."
				);

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
			videos = videos.Where(
				x => x.Visibility == VideoVisibility.Public && x.ProcessingProgress == ProcessingProgress.Ready
			);
			var result = _mapper.Map<IEnumerable<VideoMetadataDto>>(videos);
			var users = (await _repository.UsersRepository.GetUsersByIds(result.Select(x => x.AuthorId)));
			_videoManager.AddAuthorNickname(result, users);
			_videoManager.AddThumbnailUri(new Uri($"{Request.Scheme}://{Request.Host}"), result);

			return StatusCode(StatusCodes.Status200OK, result);
		}

		[HttpDelete("video", Name = "deleteVideo")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> DeleteVideo([Required] string id)
		{
			var userId = GetUserIdFromToken();
			var user = await _repository.UsersRepository.GetById(userId);
			var video = await _repository.VideoRepository.GetById(id);

			if (video == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Wideo o podanym ID nie istnieje.");
			}

			// dodać logikę że jak jest administratorem to może nawet jak nie jego wideo??
			if (video.AuthorId != userId && user.UserType != UserType.Administrator)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do usunięcia wideo.");
			}

			if (
				video.ProcessingProgress == ProcessingProgress.Uploading
				|| video.ProcessingProgress == ProcessingProgress.Processing
			)
			{
				return StatusCode(
					StatusCodes.Status400BadRequest,
					"Stan wideo nie pozwala na usunięcie. Obecny stan: " + video.ProcessingProgress.ToString() + '.'
				);
			}

			string? errorMessage = await _repository.VideoRepository.DeleteVideo(id);
			if (errorMessage != null)
				return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);

			await _repository.CommentRepository.DeleteVideoComments(id);

			return StatusCode(StatusCodes.Status200OK, "Wideo usunięte pomyślnie.");
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

			return StatusCode(StatusCodes.Status200OK, "Reakcja dodana pomyślnie.");
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

			return StatusCode(StatusCodes.Status200OK, result);
		}

		[HttpGet("/user/videos", Name = "getUserVideos")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> GetUsersVideos([Required] string id)
		{
			var senderId = GetUserIdFromToken();
			var userSender = await _repository.UsersRepository.GetById(senderId);
			var user = await _repository.UsersRepository.GetById(id);

			if (user == null)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Użytkownik o podanym ID nie istnieje.");
			}

			if (user.UserType != UserType.Creator)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Użytkownik o podanym ID nie jest twórcą.");
			}

			bool hasPermission = (id == senderId || userSender.UserType == UserType.Administrator);

			var videos = await _repository.VideoRepository.GetVideosByUserId(id, hasPermission);
			var videosDto = _mapper.Map<IEnumerable<VideoMetadataDto>>(videos);
			var users = (await _repository.UsersRepository.GetUsersByIds(videosDto.Select(x => x.AuthorId)));
			_videoManager.AddAuthorNickname(videosDto, users);
			_videoManager.AddThumbnailUri(new Uri($"{Request.Scheme}://{Request.Host}"), videosDto);

			var result = new VideoListDto(videosDto);

			return StatusCode(StatusCodes.Status200OK, result);
		}

		/// <summary>
		/// Get videos from subscribed users
		/// </summary>
		/// <returns></returns>
		/// <response code="200">OK</response>
		[HttpGet("user/videos/subscribed")]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(IEnumerable<VideoMetadataDto>))]
		public async Task<IActionResult> GetVideosSubscribed()
		{
			var id = GetUserIdFromToken();
			var subscriptions = await _repository.SubscriptionsRepository.GetUserSubscriptions(id);
			var subscribedUsersIds = new SubscriptionsManager().GetSubscribedUsersIds(subscriptions);

			var videos = await _repository.VideoRepository.GetSubscribedVideos(subscribedUsersIds);
			videos = videos.Where(x => x.ProcessingProgress == ProcessingProgress.Ready);
			var videosDto = _mapper.Map<IEnumerable<VideoMetadataDto>>(videos);
			var users = (await _repository.UsersRepository.GetUsersByIds(videosDto.Select(x => x.AuthorId)));
			_videoManager.AddAuthorNickname(videosDto, users);
			_videoManager.AddThumbnailUri(new Uri($"{Request.Scheme}://{Request.Host}"), videosDto);

			return StatusCode(StatusCodes.Status200OK, new VideoListDto(videosDto));
		}
	}
}
