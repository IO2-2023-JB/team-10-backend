using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Entities.Models;

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
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _repository.UsersRepository.GetById(userId);
        }
    }
}
