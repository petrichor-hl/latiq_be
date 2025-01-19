namespace LaTiQ.Application.DTOs.Room.Req;

public class MakeRoomRequest
{
    public Guid TopicId { get; set; }

    public int Points { get; set; }

    public int Capacity {  get; set; }

    public bool IsPublic { get; set; }
}