using Entities.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.User
{
	public class UserDto : UserBaseDto, ISearchable
	{
		/// <summary>
		///     Unique identifier
		/// </summary>
		/// <example>640df935f1afe5d21b891805</example>
		[Required]
		public string Id { get; set; }

		/// <summary>
		///     Email address
		/// </summary>
		/// <example>john.doe@mail.com</example>
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		/// <summary>
		///     Account balance
		/// </summary>
		/// <example>132</example>
		public decimal? AccountBalance { get; set; }

		/// <summary>
		///     Subscriptions counter
		/// </summary>
		/// <example>12000</example>
		public int? SubscriptionsCount { get; set; }
	}
}
