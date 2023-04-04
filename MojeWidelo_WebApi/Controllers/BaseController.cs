using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MojeWidelo_WebApi.Controllers
{
	public class BaseController : ControllerBase
	{
		protected readonly IRepositoryWrapper _repository;
		protected readonly IMapper _mapper;

		public BaseController(IRepositoryWrapper repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		protected async Task<User> GetUserFromToken()
		{
			var userId = GetUserIdFromToken();
			return await _repository.UsersRepository.GetById(userId);
		}

		protected string GetUserIdFromToken()
		{
			return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
		}
	}
}
