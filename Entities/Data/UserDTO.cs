using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class UserDto : UserBaseDto
    {
        /// <summary>
        ///     Unique identifier
        /// </summary>
        /// <example>640df935f1afe5d21b891805</example>
        [Required]
        public string Id { get; set; }

        /// <summary>
        ///     Account balance
        /// </summary>
        /// <example>132</example>
        public double AccountBalance { get; set; }

        /// <summary>
        ///     Subscriptions counter
        /// </summary>
        /// <example>12000</example>
        public int SubscriptionsCount { get; set; }
    }
}
