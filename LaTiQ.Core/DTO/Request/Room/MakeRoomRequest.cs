using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.DTO.Request.Room
{
    public class MakeRoomRequest
    {
        public Guid TopicId { get; set; }

        public int Round { get; set; }

        public int Capacity {  get; set; }

        public bool IsPublic { get; set; }
    }
}
