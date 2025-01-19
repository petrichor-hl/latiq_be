namespace LaTiQ.Core.Entities
{
    public class Room
    {
        public string RoomId { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        public Topic Topic { get; set; } = null!;

        public int Points { get; set; }

        public int Capacity { get; set; }
        
        public int Turn { get; set; }

        public bool IsPublic { get; set; }
        
        public bool IsLocked { get; set; }
    }
}
