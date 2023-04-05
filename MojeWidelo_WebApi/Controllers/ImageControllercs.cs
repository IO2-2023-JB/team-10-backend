using AutoMapper;
using Contracts;
using Entities.Data.User;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MojeWidelo_WebApi.Filters;
using MojeWidelo_WebApi.Helpers;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

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
	}
}
