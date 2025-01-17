using System.Security.Claims;
using LaTiQ.Application.DTOs.User.Req;
using LaTiQ.Core.DTOs.User.Res;
using LaTiQ.Core.Identity;
using LaTiQ.WebAPI.ServiceContracts;
using Microsoft.AspNetCore.Identity;

namespace LaTiQ.WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserProfileResponse> GetProfile()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var email = principal.FindFirstValue(ClaimTypes.Email);
            
            var user = await _userManager.FindByEmailAsync(email);

            return new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                NickName = user.NickName,
                Avatar = user.Avatar,
            };
        }

        public async Task<UserProfileResponse> UpdateProfile(UpdateUserProfileRequest req)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var email = principal.FindFirstValue(ClaimTypes.Email);
            
            var user = await _userManager.FindByEmailAsync(email);
            user.NickName = req.NickName;
            user.Avatar = req.Avatar;
            
            await _userManager.UpdateAsync(user);
            
            return new UserProfileResponse
            {
                Email = user.Email,
                NickName = user.NickName,
                Avatar = user.Avatar,
            };
        }
    }
}
