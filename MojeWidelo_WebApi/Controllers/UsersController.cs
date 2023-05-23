using AutoMapper;
using Contracts;
using Entities.Data.User;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MojeWidelo_WebApi.Filters;
using MojeWidelo_WebApi.Helpers;
using Repository.Managers;
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
		private readonly UsersManager _usersManager;

		public UsersController(IRepositoryWrapper repository, IMapper mapper, UsersManager usersManager)
			: base(repository, mapper)
		{
			_usersManager = usersManager;
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
			_usersManager.AddAvatarUri(new Uri($"{Request.Scheme}://{Request.Host}"), result);
			return StatusCode(StatusCodes.Status200OK, result);
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
				return StatusCode(StatusCodes.Status404NotFound, "Użytkownik o podanym ID nie istnieje.");
			var result = _mapper.Map<UserDto>(user);

			if (result.AvatarImage != null)
			{
				Uri location = new Uri($"{Request.Scheme}://{Request.Host}");
				result.AvatarImage = location.AbsoluteUri + result.AvatarImage;
			}

			result = _usersManager.CheckPermissionToGetAccountBalance(GetUserIdFromToken(), result);

			return StatusCode(StatusCodes.Status200OK, result);
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
				return StatusCode(StatusCodes.Status404NotFound, "Użytkownik o podanym ID nie istnieje.");
			}

			if (GetUserIdFromToken() != id)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Brak uprawnień do edycji danych użytkownika.");
			}

			if (user.UserType == UserType.Creator && userDto.UserType == UserType.Simple)
			{
				var userVideos = await _repository.VideoRepository.GetVideosByUserId(id, true);

				if (userVideos.Any())
					return StatusCode(
						StatusCodes.Status400BadRequest,
						"Twórca musi usunąć wszystkie filmy przed zmianą typu konta."
					);
			}

			user = _mapper.Map<UpdateUserDto, User>(userDto, user);

			await _repository.UsersRepository.SetAvatar(user, userDto);

			var newUser = await _repository.UsersRepository.Update(id, user);
			var result = _mapper.Map<UserDto>(newUser);
			return StatusCode(StatusCodes.Status200OK, result);
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
		public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto registerDto)
		{
			var existingUser = await _repository.UsersRepository.FindUserByEmail(registerDto.Email);
			if (existingUser != null)
				return StatusCode(StatusCodes.Status409Conflict, "Konto z podanym emailem już istnieje.");

			var user = _mapper.Map<User>(registerDto);

			await _repository.UsersRepository.SetAvatar(user, registerDto);

			user = await _repository.UsersRepository.Create(user);

			await _repository.HistoryRepository.Create(new UserHistory(user.Id));
			return StatusCode(StatusCodes.Status201Created, "Konto zostało utworzone pomyślnie.");
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
			return StatusCode(StatusCodes.Status200OK, "Użytkownik został usunięty pomyślnie.");
		}

		[HttpPost, Route("login")]
		[AllowAnonymous]
		[ServiceFilter(typeof(ModelValidationFilter))]
		public async Task<IActionResult> Login([FromBody] LoginDto user)
		{
			var returnedUser = await _repository.UsersRepository.FindUserByEmail(user.Email);
			if (returnedUser == null)
				return StatusCode(StatusCodes.Status404NotFound, "Taki użytkownik nie istnieje.");
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
				return StatusCode(StatusCodes.Status200OK, new LoginResponseDto(tokenString));
			}

			return StatusCode(StatusCodes.Status401Unauthorized, "Podane dane nie są poprawne.");
		}
	}
}
