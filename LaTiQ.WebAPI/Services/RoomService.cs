using System.Security.Claims;
using LaTiQ.Application.DTOs.Room.Req;
using LaTiQ.Application.DTOs.Room.Res;
using LaTiQ.Application.Exceptions;
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

            var topic = _topicService.GetTopic(makeRoomRequest.TopicId);
            if (topic == null)
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
                Topic = topic,
                Points = makeRoomRequest.Points,
                Capacity = makeRoomRequest.Capacity,
                Turn = 0,
                IsPublic = makeRoomRequest.IsPublic,
                IsLocked = false,
            };

            _roomData.RoomInfo[room.RoomId] = room;

            return new RoomResponse
            {
                RoomId = room.RoomId,
                OwnerId = user.Id,
                Topic = new TopicResponse
                {
                    Id = topic.Id,
                    Name = topic.Name,
                    ImageUrl = topic.ImageUrl,
                },
                Points = room.Points,
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

            if (room.IsLocked)
            {
                throw new NotFoundException($"Room {roomId} đã khoá");
            }
            
            return new RoomResponse
            {
                RoomId = room.RoomId,
                OwnerId = room.OwnerId,
                Topic = new TopicResponse
                {
                    Id = room.Topic.Id,
                    Name = room.Topic.Name,
                    ImageUrl = room.Topic.ImageUrl,
                },
                Points = room.Points,
                Capacity = room.Capacity,
                IsPublic = room.IsPublic,
            };
        }
    }
}
