namespace LaTiQ.Core.Entities
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
