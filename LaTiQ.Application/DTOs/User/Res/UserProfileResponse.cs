namespace LaTiQ.Core.DTOs.User.Res;

public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
}