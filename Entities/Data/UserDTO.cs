using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public abstract class UserBaseDTO
    {
        /// <summary>
        ///     Email address
        /// </summary>
        /// <example>john.doe@mail.com</example>
        [Required(ErrorMessage = "Email is required")]
        public string Email
        {
            get; set;
        }

        /// <summary>
        ///     Nickname
        /// </summary>
        /// <example>johnny123</example>
        [Required(ErrorMessage = "Nickname is required")]
        public string Nickname
        {
            get; set;
        }

        /// <summary>
        ///     Name
        /// </summary>
        /// <example>John</example>
        [Required(ErrorMessage = "Name is required")]
        public string Name
        {
            get; set;
        }

        /// <summary>
        ///     Surname
        /// </summary>
        /// <example>Doe</example>
        [Required(ErrorMessage = "Surname is required")]
        public string Surname
        {
            get; set;
        }

        /// <summary>
        ///     User type: Simple, Creator, Administrator
        /// </summary>
        /// <example>Creator</example>
        [Required(ErrorMessage = "UserType is required")]
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
        [Required(ErrorMessage = "Id is required")]
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
    }


    public class UpdateUserDTO : UserDTO
    {
    }

    public class RegisterDTO : UserBaseDTO
    {
    }
}
