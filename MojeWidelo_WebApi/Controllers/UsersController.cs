using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MojeWidelo_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;

        public UsersController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// *Endpoint for testing*
        /// </summary>
        /// <returns>List of all users</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(IEnumerable<UserDTO>))]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _repository.UsersRepository.GetAll();
            var result = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(result);
        }

        /// <summary>
        /// Users data retrieval
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User data</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("{id}", Name = "UserById")]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(UserDTO))]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _repository.UsersRepository.GetById(id);
            var result = _mapper.Map<UserDTO>(user);
            return Ok(result);
        }
    }
}
