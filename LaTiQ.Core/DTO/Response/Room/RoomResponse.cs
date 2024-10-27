using LaTiQ.Core.DTO.Response.Topic;
using LaTiQ.Core.DTO.Response.User;
using LaTiQ.Core.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.DTO.Response.Room
{
    public class RoomResponse
    {
        public string RoomId { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        public TopicResponse Topic { get; set; } = new TopicResponse();

        public int Round { get; set; }

        public int Capacity { get; set; }

        public bool IsPublic { get; set; }
    }
}
