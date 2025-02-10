using LaTiQ.Application.DTOs.User.Req;
using LaTiQ.Application.DTOs.User.Res;

namespace LaTiQ.WebAPI.ServiceContracts
{
    public interface IUserService
    {
        public Task<UserProfileResponse> GetProfile();
        public Task<UserProfileResponse> UpdateProfile(UpdateUserProfileRequest req);
        public Task<UserFriendResponse> GetFriendList();
        public Task<bool> SendFriendRequest(Guid receiverUserId);
        public Task<bool> AcceptFriendRequest(Guid senderUserId);
        public Task<bool> RemoveFriendRequest(Guid friendId);
        public Task UpdateStatus(bool isOnline);
    }
}
