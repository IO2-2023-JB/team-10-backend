using AutoMapper;
using Contracts;
using Entities.Data;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MojeWidelo_WebApi.Filters;
using MojeWidelo_WebApi.Models;
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
        /// <response code="404">Not found</response>
        [HttpGet("user/{id}", Name = "getUserById")]
        [ServiceFilter(typeof(ObjectIdValidationFilter))]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(UserDTO))]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _repository.UsersRepository.GetById(id);
            if(user == null) return NotFound();
            var result = _mapper.Map<UserDTO>(user);
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
        [Produces(MediaTypeNames.Application.Json, Type = typeof(UserDTO))]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO userDTO)
        {
            var user = await _repository.UsersRepository.Update(userDTO.Id, _mapper.Map<User>(userDTO));
            var result = _mapper.Map<UserDTO>(user);
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
        [Produces(MediaTypeNames.Application.Json, Type = typeof(UserDTO))]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDTO registerDto)
        {
            var user = await _repository.UsersRepository.Create(_mapper.Map<User>(registerDto));
            var result = _mapper.Map<User>(user);
            return Ok(result);
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
        public IActionResult Login([FromBody] LoginModel user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request!");
            }

            if (user.Username == "NIE@MOGE" && user.Password == "ODDYCHAC")
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hasloooo1234$#@!"));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signingCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return Ok(new { Token = tokenString });
            }

            return Unauthorized();
        }
    }
}
