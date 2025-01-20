namespace LaTiQ.Application.Models;

public class UserRoom
{
    public Guid UserId { get; set; }
    
    public string UserNickName { get; set; } = string.Empty;
        
    public string UserAvatar { get; set; } = string.Empty;

    // Số điểm hiện tại của người chơi => Mặc định ban đầu = 0
    public int UserPoints { get; set; } = 0;
    
    // Số lần người chơi là Drawer => Mặc định ban đầu = 0
    public int Turn { get; set; } = 0;
        
    public string RoomId { get; set; } = string.Empty;
    
    // public CameraStatus CameraStatus { get; set; }
}