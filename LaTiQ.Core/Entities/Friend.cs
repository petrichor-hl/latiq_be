using LaTiQ.Core.Identity;

namespace LaTiQ.Core.Entities;

public enum FriendStatus
{
    Pending,
    Accepted,
}

public class Friend
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
    
    public FriendStatus Status { get; set; }
    
    // Navigation Props
    public ApplicationUser FriendUser { get; set; } = null!;
}