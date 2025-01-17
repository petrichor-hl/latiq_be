using LaTiQ.Application.DTOs.User.Req;
using LaTiQ.Core.DTOs.User.Res;

namespace LaTiQ.WebAPI.ServiceContracts
{
    public interface IUserService
    {
        public Task<UserProfileResponse> GetProfile();
        public Task<UserProfileResponse> UpdateProfile(UpdateUserProfileRequest req);
    }
}
