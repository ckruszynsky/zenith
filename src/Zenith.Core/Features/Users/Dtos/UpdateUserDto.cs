using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Core.Features.Users.Dtos
{
    public class UpdateUserDto
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public string? Password { get; set; }
        
    }
}
