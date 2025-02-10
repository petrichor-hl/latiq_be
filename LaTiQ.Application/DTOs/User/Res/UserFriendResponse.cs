namespace LaTiQ.Application.DTOs.User.Res;

public class UserFriendResponse
{
    public List<FriendResponse> SendRequests { get; set; } = new List<FriendResponse>();
    public List<FriendResponse> ReceiveRequests { get; set; } = new List<FriendResponse>();
    public List<FriendResponse> Friends { get; set; } = new List<FriendResponse>();
}