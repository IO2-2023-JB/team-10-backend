﻿using AutoMapper;
using Contracts;
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

#warning dodać logikę ProcessingProgress

			// mapowanie i uzupełnienie danych
			var video = _mapper.Map<VideoMetadata>(videoUploadDto);
			video.UploadDate = video.EditDate = DateTime.Now;
			video.AuthorId = user.Id;
			video.AuthorNickname = user.Nickname;

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
			// pobieram stare dane, potem mapuje te pola które nie są nullami + omijam te immutable (patrz MappingProfile.cs)
			var video = await _repository.VideoRepository.GetById(id);

			if (video == null)
			{
				return NotFound();
			}

			if (GetUserIdFromToken() != video.AuthorId)
			{
				return Forbid("No permissions to edit video metadata");
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
				return Forbid("No permissions to get video metadata");
			}

			var result = _mapper.Map<VideoMetadataDto>(video);
			return Ok(result);
		}
	}
}