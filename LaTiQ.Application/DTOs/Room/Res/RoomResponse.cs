using LaTiQ.Core.DTOs.Topic.Res;

namespace LaTiQ.Application.DTOs.Room.Res;

public class RoomResponse
{
    public string RoomId { get; set; } = string.Empty;

    public Guid OwnerId { get; set; }

    public TopicResponse Topic { get; set; } = null!;

    public int Points { get; set; }

    public int Capacity { get; set; }
}