using System.Security.Claims;
using System.Web;
using LaTiQ.Application.DTOs.Account.Req;
using LaTiQ.Application.DTOs.Account.Res;
using LaTiQ.Application.Exceptions;
using LaTiQ.Core.DTOs.Account.Req;
using LaTiQ.Core.Identity;
using LaTiQ.Core.ServiceContracts;
using LaTiQ.WebAPI.ServiceContracts;
using Microsoft.AspNetCore.Identity;

namespace LaTiQ.WebAPI.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    // private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly IJwtService _jwtService;
    private readonly IEmailSender _emailSender;

    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public AccountService(
        UserManager<ApplicationUser> userManager, 
        RoleManager<ApplicationRole> roleManager, 
        SignInManager<ApplicationUser> signInManager, 
        IJwtService jwtService, IEmailSender emailSender, 
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _userManager = userManager;
        // _roleManager = roleManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _emailSender = emailSender;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<Guid> Register(RegisterRequest registerRequest)
    {
        // Email is registered
        if (_userManager.Users.Any(u => u.Email == registerRequest.Email))
        {
            throw new InvalidModelException("Email đã tồn tại");
        }

        ApplicationUser user = new()
        {
            Email = registerRequest.Email,
            UserName = registerRequest.Email,   // UserName is used for login
            NickName = registerRequest.NickName,
            Avatar = registerRequest.Avatar,
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (result.Succeeded)
        {
            var verifyEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verifyEmailTokenEncoded = HttpUtility.UrlEncode(verifyEmailToken);
            Console.WriteLine("verifyEmailToken = " + verifyEmailToken);

            var confirmLink = $"{_configuration["LatiqUrlWeb"]}/#/confirm-email?email={registerRequest.Email}&verifyEmailToken={verifyEmailTokenEncoded}";

            //await _emailSender.SendMailAsync(registerDTO.Email, "[LaTiQ] Please confirm your email address", EmailTemplate.ConfirmEmail(registerDTO.NickName, redirectTo: confirmLink));

            return user.Id;
        }
        else
        {
            var errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description)); //error1 | error2
            throw new InvalidModelException(errorMessage);
        }
    }

    public async Task<LoginSuccessResponse> Login(LoginRequest loginRequest)
    {
        SignInResult result = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, isPersistent: false, lockoutOnFailure: false);
        ApplicationUser user = await _userManager.FindByEmailAsync(loginRequest.Email);
            
        if (result.Succeeded)
        {
            string accessToken = _jwtService.GenerateAccessToken(user);
            (string refreshToken, DateTime expirationDateTime) = _jwtService.GenerateRefreshToken();
            
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpirationDateTime = expirationDateTime;
            await _userManager.UpdateAsync(user);

            return new LoginSuccessResponse()
            {
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        else
        {
            if (result.IsNotAllowed)
            {
                throw new InvalidModelException("Đăng nhập thất bại. Email chưa được xác thực.");
            }
            throw new InvalidModelException("Đăng nhập thất bại. Vui lòng kiểm tra lại Email và Mật Khẩu.");
        }
    }

    public async Task<bool> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest)
    {
        ApplicationUser user = await _userManager.FindByEmailAsync(confirmEmailRequest.Email);

        Console.WriteLine("verifyEmailToken = " + confirmEmailRequest.VerifyEmailToken);
        var result = await _userManager.ConfirmEmailAsync(user, confirmEmailRequest.VerifyEmailToken);
        return result.Succeeded;
    }

    public async Task<RefreshTokenSuccessResponse> RefreshToken(RefreshJwtTokenRequest refreshJwtTokenRequest)
    {
        ClaimsPrincipal? principal;
        try
        {
            principal = _jwtService.GetPrincipalFromJwtToken(refreshJwtTokenRequest.AccessToken);
        }
        catch (Exception ex)
        {
            throw new InvalidModelException(ex.Message);
        }

        var email = principal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(email);

        if (user.RefreshTokenExpirationDateTime <= DateTime.UtcNow)
        {
            throw new InvalidModelException("RefreshToken không hợp lệ hoặc đã quá hạn.");
        }

        string accessToken = _jwtService.GenerateAccessToken(user);
        (string refreshToken, DateTime expirationDateTime) = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationDateTime = expirationDateTime;
        await _userManager.UpdateAsync(user);

        return new RefreshTokenSuccessResponse()
        {
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    public async Task<bool> Logout()
    {
        var userPrincipal = _httpContextAccessor.HttpContext?.User;
        var userId = userPrincipal.FindFirstValue("UserId");

        var  applicationUser = await _userManager.FindByIdAsync(userId);
        applicationUser.RefreshToken = null;
        applicationUser.RefreshTokenExpirationDateTime = null;
        applicationUser.TokenVersion++;
        await _userManager.UpdateAsync(applicationUser);
        
        // Chưa rõ tác dụng của:
        await _signInManager.SignOutAsync();

        return true;
    }
}