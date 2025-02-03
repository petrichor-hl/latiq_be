using LaTiQ.Application.DTOs.Account.Req;
using LaTiQ.Application.DTOs.Account.Res;
using LaTiQ.Core.DTOs.Account.Req;

namespace LaTiQ.WebAPI.ServiceContracts;

public interface IAccountService
{
    Task<Guid> Register(RegisterRequest registerRequest);
    
    Task<LoginSuccessResponse> Login(LoginRequest loginRequest);

    Task<bool> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest);
    
    Task<RefreshTokenSuccessResponse> RefreshToken(RefreshJwtTokenRequest refreshJwtTokenRequest);

    Task<bool> Logout();
}