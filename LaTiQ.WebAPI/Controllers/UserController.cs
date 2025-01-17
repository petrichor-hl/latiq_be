using LaTiQ.WebAPI.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LaTiQ.Application.DTOs;
using LaTiQ.Application.DTOs.User.Req;
using LaTiQ.Core.DTOs.User.Res;

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
            return Ok(ApiResult<UserProfileResponse>.Success(await _userService.GetProfile()));
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileRequest updateUserProfileRequest)
        {
            return Ok(ApiResult<UserProfileResponse>.Success(await _userService.UpdateProfile(updateUserProfileRequest)));
        }
    }
}
