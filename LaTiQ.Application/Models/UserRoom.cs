using LaTiQ.Application.Enum;

namespace LaTiQ.Application.Models;

public class UserRoom
{
    public string UserEmail { get; set; } = string.Empty;
        
    public string UserAvatar { get; set; } = string.Empty;
        
    public CameraStatus CameraStatus { get; set; }
        
    public string RoomId { get; set; } = string.Empty;
}