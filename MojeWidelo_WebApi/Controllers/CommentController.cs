using AutoMapper;
using Contracts;
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

		[HttpGet("comment", Name = "getComment")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> GetComment([Required] string id)
		{
			if (await _repository.VideoRepository.GetById(id) == null)
				return NotFound();

			var comments = (await _repository.CommentRepository.GetAll()).Where((x) => x.VideoId == id).ToArray();

			return Ok(comments);
		}

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
	}
}
