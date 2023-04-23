using AutoMapper;
using Contracts;
using Entities.Data.Comment;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using Repository.Managers;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MojeWidelo_WebApi.Controllers
{
	[ApiController]
	public class CommentController : BaseController
	{
		private readonly CommentManager _commentManager;

		public CommentController(IRepositoryWrapper repository, IMapper mapper, CommentManager manager)
			: base(repository, mapper)
		{
			_commentManager = manager;
		}

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
			var user = await GetUserFromToken();
			if (video == null)
				return StatusCode(StatusCodes.Status404NotFound, "Nie znaleziono filmu o podanym identyfikatorze.");
			if (
				video.Visibility == VideoVisibility.Private
				&& user.Id != video.AuthorId
				&& user.UserType != UserType.Administrator
			)
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do komentarzy filmu.");

			var comments = await _repository.CommentRepository.GetVideoComments(id);
			var users = (await _repository.UsersRepository.GetUsersByIds(comments.Select(x => x.AuthorId))).ToHashSet();

			var commentsDto = _commentManager.CreateCommentArray(comments, users);

			return StatusCode(StatusCodes.Status200OK, commentsDto);
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

			return StatusCode(StatusCodes.Status200OK, "Komentarz dodany pomyślnie.");
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
				return StatusCode(StatusCodes.Status404NotFound, "Komentarz o podanym ID nie istnieje.");

			var user = await GetUserFromToken();
			if (comment.AuthorId != user.Id && user.UserType != UserType.Administrator)
				return StatusCode(StatusCodes.Status401Unauthorized, "Brak uprawnień do usunięcia komentarza.");

			_repository.CommentRepository.DeleteCommentResponses(id);
			await _repository.CommentRepository.Delete(id);

			return StatusCode(StatusCodes.Status200OK, "Komentarz usunięty pomyślnie.");
		}

		/// <summary>
		/// All responses to particular comment retrieval
		/// </summary>
		/// <param name="id"></param>
		/// <returns>Array of comments</returns>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="403">Forbidden</response>
		/// <response code="404">Not found</response>
		[HttpGet("comment/response", Name = "getCommentResponse")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> GetCommentResponse([Required] string id)
		{
			var comment = await _repository.CommentRepository.GetById(id);
			var video = await _repository.VideoRepository.GetById(comment.VideoId);
			var user = await GetUserFromToken();
			if (
				video.Visibility == VideoVisibility.Private
				&& user.Id != video.AuthorId
				&& user.UserType != UserType.Administrator
			)
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do odpowiedzi do komentarza.");
			if (comment == null)
				return StatusCode(
					StatusCodes.Status404NotFound,
					"Nie znaleziono komentarza o podanym identyfikatorze."
				);
			if (!comment.HasResponses)
				return StatusCode(StatusCodes.Status200OK, new CommentDto[0]);

			var comments = await _repository.CommentRepository.GetCommentResponses(id);
			var users = (await _repository.UsersRepository.GetUsersByIds(comments.Select(x => x.AuthorId))).ToHashSet();
			var commentsDto = _commentManager.CreateCommentArray(comments, users);

			return StatusCode(StatusCodes.Status200OK, commentsDto);
		}

		/// <summary>
		/// Adding a response to a comment
		/// </summary>
		/// <param name="id"></param>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		[HttpPost("comment/response", Name = "addCommentResponse")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		[Consumes("text/plain")]
		public async Task<IActionResult> AddCommentResponse([Required] string id)
		{
			string content;
			using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
			{
				content = await reader.ReadToEndAsync();
			}

			var user = await GetUserFromToken();
			var originComment = await _repository.CommentRepository.GetById(id);
			await _repository.CommentRepository.Create(new Comment(originComment.VideoId, user.Id, content, id));
			originComment.HasResponses = true;
			await _repository.CommentRepository.Update(id, originComment);

			return StatusCode(StatusCodes.Status200OK, "Odpowiedź dodana pomyślnie.");
		}
	}
}
