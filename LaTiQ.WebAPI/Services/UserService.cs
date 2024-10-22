using LaTiQ.Core.DTO.Request.User;
using LaTiQ.Core.DTO.Response.User;
using LaTiQ.Core.Identity;
using LaTiQ.WebAPI.ServiceContracts;
using Microsoft.AspNetCore.Identity;

namespace LaTiQ.WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserProfileResponse> GetProfile(string email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);

            return new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                NickName = user.NickName,
                Avatar = user.Avatar,
            };
        }

        public async Task<UserProfileResponse?> UpdateProfile(string email, UpdateUserProfileRequest req)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

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
                return new UserProfileResponse
                {
                    Email = user.Email,
                    NickName = user.NickName,
                    Avatar = user.Avatar,
                };
            }
            else
            {
                return null;
            }
        }
    }
}
