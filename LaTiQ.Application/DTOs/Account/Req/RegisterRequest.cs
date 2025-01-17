using System.ComponentModel.DataAnnotations;

namespace LaTiQ.Core.DTOs.Account.Req;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string NickName { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Avatar { get; set; } = string.Empty;
}