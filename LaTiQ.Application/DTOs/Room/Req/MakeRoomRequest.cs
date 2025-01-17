namespace LaTiQ.Core.DTOs.Room.Req;

public class MakeRoomRequest
{
    public Guid TopicId { get; set; }

    public int Round { get; set; }

    public int Capacity {  get; set; }

    public bool IsPublic { get; set; }
}