using LaTiQ.Core.DTO.Request.Room;
using LaTiQ.Core.DTO.Response.Room;
using LaTiQ.Core.DTO.Response.Topic;
using LaTiQ.Core.DTO.Response.User;
using LaTiQ.Core.Entities.Room;
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
        private readonly IUserService _userService;

        private readonly RoomData _roomData;

        public RoomService(UserManager<ApplicationUser> userManager, IUserService userService, ITopicService topicService, RoomData roomData)
        {
            _userManager = userManager;
            _topicService = topicService;
            _userService = userService;

            _roomData = roomData;
        }

        public async Task<RoomResponse?> MakeRoom(string email, MakeRoomRequest req)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);

            TopicResponse? topicResponse = _topicService.GetTopic(req.TopicId);
            if (topicResponse == null)
            {
                return null;
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
                TopicId = req.TopicId,
                Round = req.Round,
                Capacity = req.Capacity,
                IsPublic = req.IsPublic
            };

            _roomData.RoomInfo[room.RoomId] = room;

            RoomResponse roomResponse = new RoomResponse
            {
                RoomId = room.RoomId,
                OwnerId = room.OwnerId,
                Topic = topicResponse,
                Round = room.Round,
                Capacity = room.Capacity,
                IsPublic = room.IsPublic,
            };

            return roomResponse;
        }

        public RoomResponse? GetRoom(string roomId)
        {
            if (_roomData.RoomInfo.TryGetValue(roomId, out Room? room))
            {
                RoomResponse roomResponse = new RoomResponse
                {
                    RoomId = room.RoomId,
                    OwnerId = room.OwnerId,
                    Topic = _topicService.GetTopic(room.TopicId)!,
                    Round = room.Round,
                    Capacity = room.Capacity,
                    IsPublic = room.IsPublic,
                };

                return roomResponse;
            }
            else
            {
                return null;
            }
        }
    }
}
