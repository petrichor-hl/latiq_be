using LaTiQ.Core.DTO.Request.User;
using LaTiQ.Core.DTO.Response.User;

namespace LaTiQ.WebAPI.ServiceContracts
{
    public interface IUserService
    {
        public Task<UserProfileResponse> GetProfile(string email);
        public Task<UserProfileResponse?> UpdateProfile(string email, UpdateUserProfileRequest req);
    }
}
