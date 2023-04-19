using AutoMapper;
using Contracts;
using Entities.Data.Comment;
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
		/// <response code="404">Not found</response>
		[HttpGet("comment", Name = "getComment")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> GetComment([Required] string id)
		{
			if (await _repository.VideoRepository.GetById(id) == null)
				return NotFound();

			var commentsDto = new List<CommentDto>();
			(await _repository.CommentRepository.GetAll())
				.Where((x) => x.VideoId == id)
				.ToList()
				.ForEach(x => commentsDto.Add(_mapper.Map<Comment, CommentDto>(x)));

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
			await _repository.CommentRepository.Create(
				new Comment(id, user.Id, content, user.AvatarImage, user.Nickname)
			);

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
			if (await _repository.CommentRepository.GetById(id) == null)
				return BadRequest();

			await _repository.CommentRepository.Delete(id);

			return Ok("Komentarz usunięto pomyślnie.");
		}
	}
}
