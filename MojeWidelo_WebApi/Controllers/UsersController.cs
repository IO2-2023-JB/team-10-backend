using AutoMapper;
using Contracts;
using Entities.Data;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MojeWidelo_WebApi.Filters;
using MojeWidelo_WebApi.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

namespace MojeWidelo_WebApi.Controllers
{
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
        [HttpGet("getAll", Name = "GetAll")]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(IEnumerable<UserDto>))]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _repository.UsersRepository.GetAll();
            var result = _mapper.Map<IEnumerable<UserDto>>(users);
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
        /// <response code="404">Not found</response>
        [HttpGet("user/{id}", Name = "getUserById")]
        [ServiceFilter(typeof(ObjectIdValidationFilter))]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(UserDto))]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _repository.UsersRepository.GetById(id);
            if (user == null)
                return NotFound();
            var result = _mapper.Map<UserDto>(user);
            return Ok(result);
        }

        /// <summary>
        /// Users data editing
        /// </summary>
        /// <returns>User data</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPut("user", Name = "updateUser")]
        [ServiceFilter(typeof(ModelValidationFilter))]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(UserDto))]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto userDto)
        {
            var user = await _repository.UsersRepository.Update(
                userDto.Id,
                _mapper.Map<User>(userDto)
            );
            var result = _mapper.Map<UserDto>(user);
            return Ok(result);
        }

        /// <summary>
        /// User registration
        /// </summary>
        /// <param name="registerDto">User data</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpPost("register", Name = "registerUser")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ModelValidationFilter))]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            var user = await _repository.UsersRepository.Create(_mapper.Map<User>(registerDto));
            var result = _mapper.Map<User>(user);
            return Ok();
        }

        /// <summary>
        /// User account deletion
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete("user/{id}", Name = "deleteUser")]
        [ServiceFilter(typeof(ObjectIdValidationFilter))]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _repository.UsersRepository.Delete(id);
            return Ok();
        }

        [HttpPost, Route("login")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ModelValidationFilter))]
        public async Task<IActionResult> Login([FromBody] LoginDto user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request!");
            }

            var returnedUser = await _repository.UsersRepository.FindUserByEmail(user.Email);
            if (returnedUser == null)
                return NotFound();
            string password = returnedUser.Password;

            if (HashHelper.ValidatePassword(user.Password, password))
            {
                var secretKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("hasloooo1234$#@!")
                );
                var signingCredentials = new SigningCredentials(
                    secretKey,
                    SecurityAlgorithms.HmacSha256
                );

                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: new List<Claim>(),
                    signingCredentials: signingCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return Ok(new LoginResponseDto(tokenString));
            }

            return Unauthorized();
        }
    }
}
