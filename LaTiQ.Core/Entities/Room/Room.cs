﻿using LaTiQ.Core.DTO.Request.Room;
using LaTiQ.Core.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.Entities.Room
{
    public class Room
    {
        public string RoomId { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        public Guid TopicId { get; set; }

        public int Round { get; set; }

        public int Capacity { get; set; }

        public bool IsPublic { get; set; }
    }
}
