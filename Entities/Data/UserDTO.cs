using Entities.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public abstract class UserBaseDTO
    {
        /// <summary>
        ///     Email address
        /// </summary>
        /// <example>john.doe@mail.com</example>
        [Required]
        [EmailAddress]
        public string Email
        {
            get; set;
        }

        /// <summary>
        ///     Nickname
        /// </summary>
        /// <example>johnny123</example>
        [Required]
        public string Nickname
        {
            get; set;
        }

        /// <summary>
        ///     Name
        /// </summary>
        /// <example>John</example>
        [Required]
        public string Name
        {
            get; set;
        }

        /// <summary>
        ///     Surname
        /// </summary>
        /// <example>Doe</example>
        [Required]
        public string Surname
        {
            get; set;
        }

        /// <summary>
        ///     User type: Simple, Creator, Administrator
        /// </summary>
        /// <example>Creator</example>
        [EnumDataType(typeof(UserType))]
        public UserType UserType
        {
            get; set;
        }
    }

    public class UserDTO : UserBaseDTO
    {
        /// <summary>
        ///     Unique identifier
        /// </summary>
        /// <example>640df935f1afe5d21b891805</example>
        [Required]
        public string Id
        {
            get; set;
        }

        /// <summary>
        ///     Account balance
        /// </summary>
        /// <example>132</example>
        public double AccountBalance
        {
            get; set;
        }

        /// <summary>
        ///     Subscriptions counter
        /// </summary>
        /// <example>12000</example>
        public int SubscriptionsCount
        {
            get; set;
        }

        // TODO: password should be there? it is in registerDTO
        public string Password
        {
            get; set;
        }
    }

    public class UpdateUserDTO : UserDTO
    {
    }

    public class RegisterDTO : UserBaseDTO
    {
        /// <summary>
        ///     Password
        /// </summary>
        /// <example>password123</example>
        [Required]
        public string Password
        {
            get; set;
        }
    }
}
