using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
	public class RegisterRequestDto : UserBaseDto
	{
		/// <summary>
		///     Password
		/// </summary>
		/// <example>password123</example>
		[Required]
		public string Password { get; set; }
	}

	public class RegisterResponseDto
	{
		public RegisterResponseDto(string message)
		{
			Message = message;
		}

		/// <summary>
		/// Info about registration process
		/// </summary>
		/// <returns>
		///     true if account was created
		///     false if email is already used
		/// </returns>
		public string Message { get; set; }
	}
}
