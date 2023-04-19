using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;

namespace MojeWidelo_WebApi.Controllers
{
	[ApiController]
	public class ImageController : BaseController
	{
		public ImageController(IRepositoryWrapper repository, IMapper mapper)
			: base(repository, mapper) { }

		[HttpGet("avatar/{id}", Name = "getAvatar")]
		[AllowAnonymous]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> GetAvatar(string id)
		{
			var bytes = await _repository.UsersRepository.GetAvatarBytes(id);

			return File(bytes, "image/jpeg");
		}

		[HttpGet("thumbnail/{id}", Name = "GetThumbnail")]
		[AllowAnonymous]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> GetThumbnail(string id)
		{
			var bytes = await _repository.VideoRepository.GetThumbnailBytes(id);
			var contentType = _repository.VideoRepository.GetThumbnailContentType(id);

			return File(bytes, contentType);
		}
	}
}
