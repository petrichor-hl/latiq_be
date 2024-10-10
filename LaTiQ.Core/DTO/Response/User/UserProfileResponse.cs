using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.DTO.Response.User
{
    public class UserProfileResponse
    {
        public string Email { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}
