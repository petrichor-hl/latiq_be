using System.Security.Claims;
using LaTiQ.Application.Exceptions;
using LaTiQ.Core.DTOs.Room.Req;
using LaTiQ.Core.DTOs.Room.Res;
using LaTiQ.Core.DTOs.Topic.Res;
using LaTiQ.Core.Entities;
using LaTiQ.Core.Identity;
using LaTiQ.WebAPI.ServiceContracts;
using LaTiQ.WebAPI.Singletons;
using Microsoft.AspNetCore.Identity;

namespace LaTiQ.WebAPI.Services
{
    public class RoomService : IRoomService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITopicService _topicService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private readonly RoomData _roomData;

        public RoomService(
            UserManager<ApplicationUser> userManager, 
            IUserService userService, 
            ITopicService topicService, 
            IHttpContextAccessor httpContextAccessor, 
            RoomData roomData
            )
        {
            _userManager = userManager;
            _topicService = topicService;
            _httpContextAccessor = httpContextAccessor;
            _roomData = roomData;
        }

        public async Task<RoomResponse> MakeRoom(MakeRoomRequest makeRoomRequest)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var email = principal.FindFirstValue(ClaimTypes.Email);
            
            var user = await _userManager.FindByEmailAsync(email);

            var topicResponse = _topicService.GetTopic(makeRoomRequest.TopicId);
            if (topicResponse == null)
            {
                throw new NotFoundException($"Không tìm thấy Topic {makeRoomRequest.TopicId}");
            }

            int currentCount = _roomData.UniqueNumbers.Count;
            int hashCode;
            do
            {
                hashCode = Guid.NewGuid().GetHashCode();
                if (hashCode < 0)
                {
                    hashCode *= -1;
                }
                _roomData.UniqueNumbers.Add(hashCode);
            } while (_roomData.UniqueNumbers.Count == currentCount);


            Room room = new()
            {
                RoomId = hashCode.ToString(),
                OwnerId = user.Id,
                TopicId = makeRoomRequest.TopicId,
                Round = makeRoomRequest.Round,
                Capacity = makeRoomRequest.Capacity,
                IsPublic = makeRoomRequest.IsPublic
            };

            _roomData.RoomInfo[room.RoomId] = room;

            return new RoomResponse
            {
                RoomId = room.RoomId,
                OwnerId = room.OwnerId,
                Topic = topicResponse,
                Round = room.Round,
                Capacity = room.Capacity,
                IsPublic = room.IsPublic,
            };
        }

        public RoomResponse GetRoom(string roomId)
        {
            var room = _roomData.RoomInfo.GetValueOrDefault(roomId);
            if (room == null)
            {
                throw new NotFoundException($"Không tìm thấy Room {roomId}");
            }
            
            return new RoomResponse
            {
                RoomId = room.RoomId,
                OwnerId = room.OwnerId,
                Topic = _topicService.GetTopic(room.TopicId)!,
                Round = room.Round,
                Capacity = room.Capacity,
                IsPublic = room.IsPublic,
            };
        }
    }
}
