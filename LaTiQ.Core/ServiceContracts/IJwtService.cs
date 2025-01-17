using LaTiQ.Core.Identity;
using System.Security.Claims;
using LaTiQ.Application.Models;

namespace LaTiQ.Core.ServiceContracts;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user);
    (string refreshToken, DateTime expirationDateTime) GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromJwtToken(string token);
}
