using Entities.Enums;

namespace Entities.Models
{
    public class UserDTO
    {
        /// <summary>
        ///     Unique identifier
        /// </summary>
        /// <example>640df935f1afe5d21b891805</example>
        public string Id
        {
            get; set;
        }

        /// <summary>
        ///     Email address
        /// </summary>
        /// <example>john.doe@mail.com</example>
        public string Email
        {
            get; set;
        }

        /// <summary>
        ///     Nickname
        /// </summary>
        /// <example>johnny123</example>
        public string Nickname
        {
            get; set;
        }

        /// <summary>
        ///     Name
        /// </summary>
        /// <example>John</example>
        public string Name
        {
            get; set;
        }

        /// <summary>
        ///     Surname
        /// </summary>
        /// <example>Doe</example>
        public string Surname
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
        ///     User type: Simple, Creator, Administrator
        /// </summary>
        /// <example>Creator</example>
        public UserType UserType
        {
            get; set;
        }
    }
}
