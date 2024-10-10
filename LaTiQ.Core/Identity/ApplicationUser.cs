using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string NickName { get; set; } = string.Empty;
        public string Avatar {  get; set; } = string.Empty;
        public int TokenVersion { get; set; }   // Default equal to 0
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpirationDateTime { get; set; }
    }
}
