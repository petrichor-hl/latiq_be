using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.DTO.Request.Room
{
    public class UserRoom
    {
        public string UserEmail { get; set; } = string.Empty;
        
        public string UserAvatar { get; set; } = string.Empty;
        
        public string RoomId { get; set; } = string.Empty;
    }
}
