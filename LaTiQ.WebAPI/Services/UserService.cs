using System.Security.Claims;
using LaTiQ.Application.DTOs.User.Req;
using LaTiQ.Application.DTOs.User.Res;
using LaTiQ.Application.Exceptions;
using LaTiQ.Core.Entities;
using LaTiQ.Infrastructure.DatabaseContext;
using LaTiQ.WebAPI.Hubs;
using LaTiQ.WebAPI.ServiceContracts;
using LaTiQ.WebAPI.Singletons;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LaTiQ.WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private readonly UserConnection _userConnection;
        private readonly IHubContext<GlobalHub> _hubContext;
        
        public UserService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IHubContext<GlobalHub> hubContext, UserConnection userConnection)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
            _userConnection = userConnection;
        }

        public async Task<UserProfileResponse> GetProfile()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var userId = Guid.Parse(principal.FindFirstValue("UserId"));
            
            var user = await _dbContext.Users.SingleAsync(u => u.Id == userId);

            return new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                NickName = user.NickName,
                Avatar = user.Avatar,
                Experience = user.Experience,
            };
        }

        public async Task<UserProfileResponse> UpdateProfile(UpdateUserProfileRequest req)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var userId = Guid.Parse(principal.FindFirstValue("UserId"));
            
            var user = await _dbContext.Users.SingleAsync(u => u.Id == userId);
            
            user.NickName = req.NickName;
            user.Avatar = req.Avatar;
            
            await _dbContext.SaveChangesAsync();
            
            return new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                NickName = user.NickName,
                Avatar = user.Avatar,
            };
        }

        public async Task<UserFriendResponse> GetFriendList()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var userId = Guid.Parse(principal.FindFirstValue("UserId"));
            
            var sendRequests = await _dbContext.Friends
                .Where(f => f.UserId == userId && f.Status == FriendStatus.Pending)
                .Include(f => f.FriendUser)
                .Select(f => new FriendResponse
                {
                    FriendId = f.Id,
                    UserId = f.FriendId,
                    Email = f.FriendUser.Email,
                    NickName = f.FriendUser.NickName,
                    Avatar = f.FriendUser.Avatar,
                    IsOnline = false,   
                })
                .ToListAsync();
            
            var receiveRequests = await _dbContext.Friends
                .Where(f => f.FriendId == userId && f.Status == FriendStatus.Pending)
                .Include(f => f.User)
                .Select(f => new FriendResponse
                {
                    FriendId = f.Id,
                    UserId = f.UserId,
                    Email = f.User.Email,
                    NickName = f.User.NickName,
                    Avatar = f.User.Avatar,
                    IsOnline = false,
                })
                .ToListAsync();

            var friends =
                await _dbContext.Friends
                    .Where(f => f.UserId == userId && f.Status == FriendStatus.Accepted)
                    .Include(f => f.FriendUser)
                    .Select(f => new FriendResponse
                    {
                        FriendId = f.Id,
                        UserId = f.FriendId,
                        Email = f.FriendUser.Email,
                        NickName = f.FriendUser.NickName,
                        Avatar = f.FriendUser.Avatar,
                        IsOnline = f.FriendUser.IsOnline,
                    })
                    .ToListAsync();
            
            
            
            return new UserFriendResponse
            {
                SendRequests = sendRequests,
                ReceiveRequests = receiveRequests,
                Friends = friends,
            };
        }

        public async Task<bool> SendFriendRequest(Guid receiverUserId)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var senderUserId = Guid.Parse(principal.FindFirstValue("UserId"));

            await _dbContext.Friends.AddAsync(new Friend
            {
                UserId = senderUserId,
                FriendId = receiverUserId,
                Status = FriendStatus.Pending,
            });
            await _dbContext.SaveChangesAsync();

            if (_userConnection.Mapping.TryGetValue(receiverUserId, out string? receiverConnectionId))
            {
                var senderProfile = await GetProfile();
                await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveFriendRequest", senderProfile);
            }
            
            return true;
        }

        public async Task<bool> AcceptFriendRequest(Guid senderUserId)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var receiverReceiverId = Guid.Parse(principal.FindFirstValue("UserId"));
            
            var friend = await _dbContext.Friends
                .SingleOrDefaultAsync(f => f.UserId == senderUserId && f.FriendId == receiverReceiverId);

            if (friend == null)
            {
                throw new NotFoundException($"Không tìm thấy yêu cầu kết bạn được gửi từ {senderUserId} đến tài khoản này.");
            }
            // A -> B -> Accepted
            friend.Status = FriendStatus.Accepted;

            // B -> A -> Accepted
            await _dbContext.Friends.AddAsync(new Friend
            {
                UserId = receiverReceiverId,
                FriendId = senderUserId,
                Status = FriendStatus.Accepted,
            });
            
            await _dbContext.SaveChangesAsync();
            
            if (_userConnection.Mapping.TryGetValue(senderUserId, out string? senderConnectionId))
            {
                var receiverProfile = await GetProfile();
                await _hubContext.Clients.Client(senderConnectionId).SendAsync("AcceptFriendRequest", receiverProfile);
            }
            
            return true;
        }

        public async Task<bool> RemoveFriendRequest(Guid friendId)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var userId = Guid.Parse(principal.FindFirstValue("UserId"));
            
            var friend = await _dbContext.Friends
                .SingleOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);

            if (friend == null)
            {
                throw new NotFoundException($"Không tìm thấy quan hệ Friend giữa tài khoản này và User {friendId}");
            }
            
            // Remove A -> B
            _dbContext.Friends.Remove(friend);
                
            // Remove B -> A
            var inverseFriend = await _dbContext.Friends
                .SingleOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId);
            _dbContext.Friends.Remove(inverseFriend!);
            
            await _dbContext.SaveChangesAsync();
            
            return true;
        }

        public async Task UpdateStatus(bool isOnline)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var userId = Guid.Parse(principal.FindFirstValue("UserId")) ;
            
            var user = await _dbContext.Users.FirstAsync(u => u.Id == userId);
            user.IsOnline = isOnline;
            
            await _dbContext.SaveChangesAsync();
        }
    }
}
