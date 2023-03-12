using Entities.Enums;

namespace Entities.Models
{
    public class UserDTO
    {
        public string Id
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public string Nickname
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Surname
        {
            get; set;
        }

        public double AccountBalance
        {
            get; set;
        }

        public UserType UserType
        {
            get; set;
        }
    }
}
