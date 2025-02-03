namespace LaTiQ.Application.DTOs.User.Res;

public class FriendResponse
{
    public Guid FriendId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
}