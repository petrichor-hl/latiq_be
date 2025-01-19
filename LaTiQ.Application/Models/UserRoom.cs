namespace LaTiQ.Application.Models;

public class UserRoom
{
    public Guid UserId { get; set; }
    
    public string UserNickName { get; set; } = string.Empty;
        
    public string UserAvatar { get; set; } = string.Empty;
        
    // public CameraStatus CameraStatus { get; set; }
        
    public string RoomId { get; set; } = string.Empty;
}