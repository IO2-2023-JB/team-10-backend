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

        public UsersController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Users data retrieval
        /// </summary>
        /// <returns>List of all users</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(IEnumerable<User>))]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _repository.UsersRepository.GetAll();
            return Ok(users);
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
        [Produces(MediaTypeNames.Application.Json, Type = typeof(User))]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _repository.UsersRepository.GetById(id);
            return Ok(user);
        }
    }
}
