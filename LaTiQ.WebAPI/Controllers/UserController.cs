using LaTiQ.WebAPI.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using LaTiQ.Application.DTOs;
using LaTiQ.Application.DTOs.User.Req;
using LaTiQ.Application.DTOs.User.Res;

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
        
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriendList()
        {
            return Ok(ApiResult<List<FriendResponse>>.Success(await _userService.GetFriendList()));
        }
        
        [HttpPost("send-friend-request/{receiverUserId:guid}")]
        public async Task<IActionResult> SendFriendRequest(Guid receiverUserId)
        {
            return Ok(ApiResult<bool>.Success(await _userService.SendFriendRequest(receiverUserId)));
        }
        
        [HttpPost("accept-friend-request/{senderUserId:guid}")]
        public async Task<IActionResult> AcceptFriendRequest(Guid senderUserId)
        {
            return Ok(ApiResult<bool>.Success(await _userService.AcceptFriendRequest(senderUserId)));
        }
        
        [HttpPost("remove-friend-request/{friendId:guid}")]
        public async Task<IActionResult> RemoveFriendRequest(Guid friendId)
        {
            return Ok(ApiResult<bool>.Success(await _userService.RemoveFriendRequest(friendId)));
        }
    }
}
