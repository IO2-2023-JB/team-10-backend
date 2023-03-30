using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Data
{
    public class LoginDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class LoginResponseDTO
    {
        public LoginResponseDTO(string token)
        {
            Token  = token;
        }

        public string Token { get; set; }
    }
}
