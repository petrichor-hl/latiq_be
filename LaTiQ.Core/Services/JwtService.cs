using LaTiQ.Core.Identity;
using LaTiQ.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LaTiQ.Application.Models;

namespace LaTiQ.Core.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(ApplicationUser user)
    {
        DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));
    
        Claim[] claims = new Claim[] {
            new Claim("UserId", user.Id.ToString()), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT unique ID
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), // Issued at (date and time of token generation)
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("tokenVersion", user.TokenVersion.ToString()),
        };
    
        // Create a SymmetricSecurityKey object using the key specified in the configuration.
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    
        // Create a SigningCredentials object with the security key and the HMACSHA256 algorithm.
        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    
        // Create a JwtSecurityToken object with the given issuer, audience, claims, expiration, and signing credentials.
        JwtSecurityToken tokenGenerator = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expiration,
            signingCredentials: signingCredentials
        );
    
        // Create a JwtSecurityTokenHandler object and use it to write the token as a string.
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        string accessToken = tokenHandler.WriteToken(tokenGenerator);
    
        return accessToken;
    }
    
    // Creates a refresh token (base 64 string of random numbers)
    public (string refreshToken, DateTime expirationDateTime) GenerateRefreshToken()
    {
        byte[] bytes = new byte[64];
        var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(bytes);
        
        var refreshToken = Convert.ToBase64String(bytes);
        var expirationDateTime =
            DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["RefreshToken:EXPIRATION_MINUTES"]));
        return (refreshToken, expirationDateTime) ;
    }

    public ClaimsPrincipal? GetPrincipalFromJwtToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidAudience = _configuration["Jwt:Audience"],
            ValidateIssuer = true,
            ValidIssuer = _configuration["Jwt:Issuer"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),

            ValidateLifetime = false, // should be false
        };

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("AccessToken không hợp lệ");
        }

        return principal;
        
    }
}
