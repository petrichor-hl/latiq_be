using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LaTiQ.Application.DTOs;
using LaTiQ.Core.DTOs.Account.Req;
using LaTiQ.Core.DTOs.Account.Res;
using LaTiQ.WebAPI.ServiceContracts;

namespace LaTiQ.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            return Ok(ApiResult<Guid>.Success(await _accountService.Register(registerRequest)));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            return Ok(ApiResult<LoginSuccessResponse>.Success(await _accountService.Login(loginRequest)));
        }

        [AllowAnonymous]
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest)
        {
            return Ok(ApiResult<bool>.Success(await _accountService.ConfirmEmail(confirmEmailRequest)));
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> GenerateNewAccessToken(RefreshJwtTokenRequest refreshJwtTokenRequest)
        {
            return Ok(ApiResult<RefreshTokenSuccessResponse>.Success(await _accountService.RefreshToken(refreshJwtTokenRequest)));
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok(ApiResult<bool>.Success(await _accountService.Logout()));
        }
    }
}
