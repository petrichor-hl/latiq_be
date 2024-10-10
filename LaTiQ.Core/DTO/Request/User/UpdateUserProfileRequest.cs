using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.DTO.Request.User
{
    public class UpdateUserProfileRequest
    {
        public string? Email { get; set; }
        public string? NickName { get; set; }
        public string? Avatar { get; set; }
    }
}
