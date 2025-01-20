using LaTiQ.Application.Models;

namespace LaTiQ.Core.Entities
{
    public class Room
    {
        public string RoomId { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        public Topic Topic { get; set; } = null!;
        
        public int RandomWordIndex { get; set; }

        public int Points { get; set; }

        public int Capacity { get; set; }
        
        public List<UserRoom> UsersInRoom { get; set; } = new List<UserRoom>();

        public int Turn { get; set; } = 0;
        
        public Guid DrawerId { get; set; }

        public bool IsEnd { get; set; } = false;
    }
}
