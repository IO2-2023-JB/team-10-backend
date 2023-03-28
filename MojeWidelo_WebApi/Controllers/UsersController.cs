﻿using AutoMapper;
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
	public class UsersController : BaseController
	{
		public UsersController(IRepositoryWrapper repository, IMapper mapper)
			: base(repository, mapper) { }

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
		/// <returns>User data</returns>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorized</response>
		/// <response code="404">Not found</response>
		[HttpGet("user", Name = "getUserById")]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(UserDto))]
		public async Task<IActionResult> GetUserById(string? id = null)
		{
			id ??= GetUserIdFromToken();

			var user = await _repository.UsersRepository.GetById(id);
			if (user == null)
				return NotFound();
			var result = _mapper.Map<UserDto>(user);

			result = _repository.UsersRepository.CheckPermissionToGetAccountBalance(GetUserIdFromToken(), result);

			return Ok(result);
		}

		/// <summary>
		/// Users data editing
		/// </summary>
		/// <param name="id"></param>
		/// <param name="userDto"></param>
		/// <returns>User data</returns>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorized</response>
		/// <response code="403">Forbidden</response>
		[HttpPut("user", Name = "updateUser")]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(UserDto))]
		public async Task<IActionResult> UpdateUser(string? id, [FromBody] UpdateUserDto userDto)
	{
			id ??= GetUserIdFromToken();
			var user = await _repository.UsersRepository.GetById(id);

			if (user == null)
			{
				return NotFound("Użytkownik o podanym id nie istnieje.");
			}

			if (GetUserIdFromToken() != id)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do edycji danych użytkownika.");
			}

			user = _mapper.Map<UpdateUserDto, User>(userDto, user);

			 if (userDTO.AvatarImage != null)
            {
                user.AvatarImage = await _repository.UsersRepository.UploadAvatar(user, userDTO.AvatarImage);
            }


			var newUser = await _repository.UsersRepository.Update(id, user);
			var result = _mapper.Map<UserDto>(newUser);
			return Ok(result);
		}

		/// <summary>
		/// User registration
		/// </summary>
		/// <param name="registerDto">User data</param>
		/// <returns></returns>
		/// <response code="201">Created</response>
		/// <response code="400">Bad request</response>
		/// <response code="409">Conflict</response>
		[HttpPost("register", Name = "registerUser")]
		[AllowAnonymous]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(RegisterResponseDto))]
		public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto registerDto)
		{
			var existingUser = await _repository.UsersRepository.FindUserByEmail(registerDto.Email);
			if (existingUser != null)
				return StatusCode(StatusCodes.Status409Conflict, new RegisterResponseDto("Account already exists."));

			

			var user = await _repository.UsersRepository.Create(_mapper.Map<User>(registerDto));
			
			if (registerDto.AvatarImage != null)
            {
                // upload avatara, uzupełnienie info w userze, update usera
                user.AvatarImage = await _repository.UsersRepository.UploadAvatar(user, registerDto.AvatarImage);
                user = await _repository.UsersRepository.Update(user.Id, user);
            }


			return StatusCode(StatusCodes.Status201Created, new RegisterResponseDto("Account created successfully."));
		}

		/// <summary>
		/// User account deletion
		/// </summary>
		/// <response code="200">OK</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorized</response>
		[HttpDelete("user", Name = "deleteUser")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> DeleteUser([Required] string id)
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

			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Role, returnedUser.UserType.ToString()),
				new Claim(ClaimTypes.NameIdentifier, returnedUser.Id),
				new Claim(ClaimTypes.Name, returnedUser.Nickname),
			};

			if (HashHelper.ValidatePassword(user.Password, password))
			{
				var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hasloooo1234$#@!"));
				var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

				var tokenOptions = new JwtSecurityToken(
					issuer: "https://localhost:5001",
					audience: "https://localhost:5001",
					claims: claims,
					signingCredentials: signingCredentials
				);

				var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
				return Ok(new LoginResponseDto(tokenString));
			}

			return Unauthorized();
		}
	}
}
