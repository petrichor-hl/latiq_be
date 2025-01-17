namespace LaTiQ.Core.DTOs.Account.Res;

public class LoginSuccessResponse
{
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}