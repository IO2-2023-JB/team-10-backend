using System.ComponentModel.DataAnnotations;

namespace Entities.Data.User
{
	public class RegisterRequestDto : UserBaseDto
	{
		/// <summary>
		///     Email address
		/// </summary>
		/// <example>john.doe@mail.com</example>
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		/// <summary>
		///     Password
		/// </summary>
		/// <example>password123</example>
		[Required]
		public string Password { get; set; }
	}
}
