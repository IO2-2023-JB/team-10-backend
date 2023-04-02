using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
	public abstract class UserBaseDto
	{
		/// <summary>
		///     Email address
		/// </summary>
		/// <example>john.doe@mail.com</example>
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		/// <summary>
		///     Nickname
		/// </summary>
		/// <example>johnny123</example>
		[Required]
		public string Nickname { get; set; }

		/// <summary>
		///     Name
		/// </summary>
		/// <example>John</example>
		[Required]
		public string Name { get; set; }

		/// <summary>
		///     Surname
		/// </summary>
		/// <example>Doe</example>
		[Required]
		public string Surname { get; set; }

		/// <summary>
		///     User type: Simple, Creator, Administrator
		/// </summary>
		/// <example>Creator</example>
		[EnumDataType(typeof(UserType))]
		public UserType UserType { get; set; }
	}
}
