using System.ComponentModel.DataAnnotations;

namespace LaTiQ.Application.DTOs.User.Req;

public class UpdateUserProfileRequest
{
    [Required] 
    public string NickName { get; set; } = string.Empty;
    
    [Required]
    public string Avatar { get; set; } = string.Empty;
}