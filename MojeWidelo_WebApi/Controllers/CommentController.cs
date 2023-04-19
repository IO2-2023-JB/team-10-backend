using AutoMapper;
using Contracts;
using Entities.Data.Comment;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MojeWidelo_WebApi.Controllers
{
	[ApiController]
	public class CommentController : BaseController
	{
		public CommentController(IRepositoryWrapper repository, IMapper mapper)
			: base(repository, mapper) { }

		/// <summary>
		/// All comments of particular video retrieval
		/// </summary>
		/// <param name="id"></param>
		/// <returns>Array of comments</returns>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		[HttpGet("comment", Name = "getComment")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> GetComment([Required] string id)
		{
			var video = await _repository.VideoRepository.GetById(id);
			if (video == null)
				return NotFound();
			if (video.Visibility == VideoVisibility.Private && GetUserIdFromToken() != video.AuthorId)
				return StatusCode(StatusCodes.Status403Forbidden, "No permission to access video comments.");

			var comments = await _repository.CommentRepository.GetVideoComments(id);
			var users = (await _repository.UsersRepository.GetUsersByIds(comments.Select(x => x.AuthorId))).ToHashSet();

			var commentsDto = new List<CommentDto>();
			comments.ForEach(x =>
			{
				commentsDto.Add(
					_mapper.Map<Comment, CommentDto>(
						x,
						opt =>
							opt.AfterMap(
								(src, dest) =>
								{
									var user = users.TakeWhile((y) => x.AuthorId == y.Id).First();
									dest.Nickname = user.Nickname;
									dest.AvatarImage = user.AvatarImage;
								}
							)
					)
				);
			});

			return Ok(commentsDto);
		}

		/// <summary>
		/// Adding a comment to a video
		/// </summary>
		/// <param name="id"></param>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		[HttpPost("comment", Name = "addComment")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		[Consumes("text/plain")]
		public async Task<IActionResult> AddComment([Required] string id)
		{
			string content;
			using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
			{
				content = await reader.ReadToEndAsync();
			}

			var user = await GetUserFromToken();
			await _repository.CommentRepository.Create(new Comment(id, user.Id, content));

			return Ok("Komentarz dodany pomyślnie.");
		}

		/// <summary>
		/// Comment deletion
		/// </summary>
		/// <param name="id"></param>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="404">Not found</response>
		[HttpDelete("comment", Name = "deleteComment")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> DeleteComment([Required] string id)
		{
			var comment = await _repository.CommentRepository.GetById(id);
			if (comment == null)
				return BadRequest();

			var user = await GetUserFromToken();
			if (comment.AuthorId != user.Id || user.UserType != UserType.Administrator)
				return Unauthorized();

			await _repository.CommentRepository.Delete(id);

			return Ok("Komentarz usunięto pomyślnie.");
		}
	}
}
