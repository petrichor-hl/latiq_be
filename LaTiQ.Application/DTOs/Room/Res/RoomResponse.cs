using LaTiQ.Core.DTOs.Topic.Res;

namespace LaTiQ.Core.DTOs.Room.Res;

public class RoomResponse
{
    public string RoomId { get; set; } = string.Empty;

    public Guid OwnerId { get; set; }

    public TopicResponse Topic { get; set; } = null!;

    public int Round { get; set; }

    public int Capacity { get; set; }

    public bool IsPublic { get; set; }
}