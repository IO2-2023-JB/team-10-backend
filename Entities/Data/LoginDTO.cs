using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public LoginResponseDto(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}
