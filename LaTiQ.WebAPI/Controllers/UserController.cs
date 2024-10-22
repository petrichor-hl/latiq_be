using LaTiQ.Core.DTO.Request.User;
using LaTiQ.Core.DTO.Response.User;
using LaTiQ.Core.Identity;
using LaTiQ.Core.ServiceContracts;
using LaTiQ.WebAPI.ServiceContracts;
using LaTiQ.WebAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LaTiQ.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-profile")]
        public async Task<IActionResult> GetProfile()
        {
            ClaimsPrincipal principal = User;
            string email = principal.FindFirstValue(ClaimTypes.Email);

            UserProfileResponse? userProfile = await _userService.GetProfile(email);

            if (userProfile == null)
            {
                return BadRequest("Email does not exist");
            }
            else
            {
                return Ok(userProfile);
            }
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileRequest req)
        {
            ClaimsPrincipal principal = User;
            string email = principal.FindFirstValue(ClaimTypes.Email);

            UserProfileResponse? userProfile = await _userService.UpdateProfile(email, req);

            if (userProfile == null)
            {
                return BadRequest("Cập nhật tài khoản không thành công.");
            }
            else
            {
                return Ok(userProfile);
            }
        }
    }
}
