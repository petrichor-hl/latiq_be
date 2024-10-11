using LaTiQ.Core.DTO.Request.Room;
using LaTiQ.Core.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.Entities.Room
{
    public class Room: MakeRoomRequest
    {
        public int RoomId { get; set; }
        public Guid OwnerId { get; set; }
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
