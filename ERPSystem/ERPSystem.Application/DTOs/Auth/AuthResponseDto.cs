using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;

        public DateTime Expiration { get; set; }

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public IList<string> Roles { get; set; } = new List<string>();
    }
}
