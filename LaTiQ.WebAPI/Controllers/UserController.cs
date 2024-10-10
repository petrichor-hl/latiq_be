using LaTiQ.Core.DTO.Request.User;
using LaTiQ.Core.DTO.Response.User;
using LaTiQ.Core.Identity;
using LaTiQ.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaTiQ.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IJwtService _jwtService;

        public UserController(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService, IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpGet("get-profile")]
        public async Task<IActionResult> GetProfile()
        {
            ClaimsPrincipal principal = User;
            string email = principal.FindFirstValue(ClaimTypes.Email);

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Email does not exist");
            }
            else
            {
                return Ok(new UserProfileResponse {
                    Email = user.Email, 
                    NickName = user.NickName, 
                    Avatar = user.Avatar, 
                });
            }
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileRequest req)
        {
            ClaimsPrincipal principal = User;
            string email = principal.FindFirstValue(ClaimTypes.Email);

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Email does not exist");
            }
            else
            {
                if (req.Email != null)
                {
                    user.Email = req.Email;
                }
                if (req.NickName != null)
                {
                    user.NickName = req.NickName;
                }
                if (req.Avatar != null)
                {
                    user.Avatar = req.Avatar;
                }

                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new UserProfileResponse
                    {
                        Email = user.Email,
                        NickName = user.NickName,
                        Avatar = user.Avatar,
                    });
                }
                else
                {
                    return BadRequest("Cập nhật thông tin không thành công.");
                }
               
            }
        }
    }
}
