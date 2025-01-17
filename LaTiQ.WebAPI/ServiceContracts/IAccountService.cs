using LaTiQ.Core.DTOs.Account.Req;
using LaTiQ.Core.DTOs.Account.Res;

namespace LaTiQ.WebAPI.ServiceContracts;

public interface IAccountService
{
    Task<Guid> Register(RegisterRequest registerRequest);
    
    Task<LoginSuccessResponse> Login(LoginRequest loginRequest);

    Task<bool> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest);
    
    Task<RefreshTokenSuccessResponse> RefreshToken(RefreshJwtTokenRequest refreshJwtTokenRequest);

    Task<bool> Logout();
}