namespace LaTiQ.Application.DTOs.Account.Req;

public class ConfirmEmailRequest
{
    public string? Email { get; set; }
    public string? VerifyEmailToken { get; set; }
}