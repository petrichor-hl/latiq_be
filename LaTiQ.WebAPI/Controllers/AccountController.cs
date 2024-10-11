using LaTiQ.Core.DTO.Request.Account;
using LaTiQ.Core.DTO.Response;
using LaTiQ.Core.Entities;
using LaTiQ.Core.Identity;
using LaTiQ.Core.ServiceContracts;
using LaTiQ.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Web;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace LaTiQ.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IJwtService _jwtService;
        private readonly IEmailSender _emailSender;

        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager, 
            SignInManager<ApplicationUser> signInManager, 
            IJwtService jwtService, IEmailSender emailSender, 
            IConfiguration configuration
            )
        {
            _userManager = userManager;
             _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errorMessage);
            }

            if (IsEmailAlreadyRegistered(registerDTO.Email))
            {
                return BadRequest("The Email is already registered");
            }

            ApplicationUser user = new()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,   // UserName is used for login
                NickName = registerDTO.NickName,
                Avatar = registerDTO.Avatar,
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                string verifyEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string verifyEmailTokenEncoded = HttpUtility.UrlEncode(verifyEmailToken);
                Console.WriteLine("verifyEmailToken = " + verifyEmailToken);

                string confirmLink = $"{_configuration["LatiqUrlWeb"]}/#/confirm-email?email={registerDTO.Email}&verifyEmailToken={verifyEmailTokenEncoded}";

                await _emailSender.SendMailAsync(registerDTO.Email, "[LaTiQ] Please confirm your email address", EmailTemplate.ConfirmEmail(registerDTO.NickName, redirectTo: confirmLink));

                return Ok("Account created! Please confirm your email in the inbox.");
            }
            else
            {
                string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description)); //error1 | error2
                return Problem(errorMessage, statusCode: 400);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            //Validation
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }


            SignInResult result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
                JwtToken jwtToken = _jwtService.CreateJwtToken(user);

                user.RefreshToken = jwtToken.RefreshToken;
                user.RefreshTokenExpirationDateTime = jwtToken.RefreshTokenExpirationDateTime;
                await _userManager.UpdateAsync(user);

                return Ok(new AuthenticationResponse
                {
                    Email = loginDTO.Email,
                    AccessToken = jwtToken.AccessToken,
                    RefreshToken = jwtToken.RefreshToken,
                });
            }
            else
            {
                return BadRequest("Email hoặc Mật khẩu không hợp lệ.");
            }
        }

        [AllowAnonymous]
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDTO confirmEmailDTO)
        {
            if (string.IsNullOrEmpty(confirmEmailDTO.Email) || string.IsNullOrEmpty(confirmEmailDTO.VerifyEmailToken))
            {
                return BadRequest("Invalid Request");
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(confirmEmailDTO.Email);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }

            Console.WriteLine("verifyEmailToken = " + confirmEmailDTO.VerifyEmailToken);
            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDTO.VerifyEmailToken);
            if (result.Succeeded)
            {
                return Ok("Email confirmation successful.");
            }
            else
            {
                return BadRequest("Email confirmation failed.");
            }
        }

        [AllowAnonymous]
        [HttpPost("generate-new-jwt-token")]
        public async Task<IActionResult> GenerateNewAccessToken(RefreshJwtTokenDTO req)
        {
            if (req.AccessToken == null) 
            {
                return BadRequest("Not Found accessToken");
            } 

            if (req.RefreshToken == null)
            {
                return BadRequest("Not Found refreshToken");
            }

            ClaimsPrincipal? principal;
            try
            {
                 principal = _jwtService.GetPrincipalFromJwtToken(req.AccessToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.GetType().FullName);
                Console.WriteLine("Message: " + ex.Message);
                return BadRequest("Invalid jwt access token");
            }

            string email = principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != req.RefreshToken || user.RefreshTokenExpirationDateTime <= DateTime.Now)
            {
                return BadRequest("Invalid refreshToken");
            }

            JwtToken jwtToken = _jwtService.CreateJwtToken(user);

            user.RefreshToken = jwtToken.RefreshToken;
            user.RefreshTokenExpirationDateTime = jwtToken.RefreshTokenExpirationDateTime;
            await _userManager.UpdateAsync(user);

            return Ok(new AuthenticationResponse
            {
                Email = email,
                AccessToken = jwtToken.AccessToken,
                RefreshToken = jwtToken.RefreshToken,
            });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            // Chưa rõ tác dụng của:
            // await _signInManager.SignOutAsync();

            ClaimsPrincipal principal = User;
            string email = principal.FindFirstValue(ClaimTypes.Email);

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Email does not exist");
            }
            else
            {
                user.RefreshToken = null;
                user.RefreshTokenExpirationDateTime = null;
                user.TokenVersion++;
                await _userManager.UpdateAsync(user);
                return Ok("The user has been logged out");
            }

        }

        private bool IsEmailAlreadyRegistered(string email)
        {
            return _userManager.Users.Any(u => u.Email == email);
        }

    }
}
