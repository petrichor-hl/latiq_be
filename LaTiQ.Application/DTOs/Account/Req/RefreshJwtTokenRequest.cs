using System.ComponentModel.DataAnnotations;

namespace LaTiQ.Application.DTOs.Account.Req;

public class RefreshJwtTokenRequest
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}