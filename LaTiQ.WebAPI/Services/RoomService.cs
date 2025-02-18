﻿using System.Security.Claims;
using LaTiQ.Application.DTOs.Room.Req;
using LaTiQ.Application.DTOs.Room.Res;
using LaTiQ.Application.Exceptions;
using LaTiQ.Core.DTOs.Topic.Res;
using LaTiQ.Core.Entities;
using LaTiQ.Core.Identity;
using LaTiQ.WebAPI.Hubs;
using LaTiQ.WebAPI.ServiceContracts;
using LaTiQ.WebAPI.Singletons;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace LaTiQ.WebAPI.Services
{
    public class RoomService : IRoomService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITopicService _topicService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private readonly RoomData _roomData;
        
        private readonly IHubContext<GlobalHub> _hubContext;
        
        private readonly Random _random = new Random();

        public RoomService(
            UserManager<ApplicationUser> userManager, 
            ITopicService topicService, 
            IHttpContextAccessor httpContextAccessor, 
            RoomData roomData, 
            IHubContext<GlobalHub> hubContext
            )
        {
            _userManager = userManager;
            _topicService = topicService;
            _httpContextAccessor = httpContextAccessor;
            
            _roomData = roomData;
            
            _hubContext = hubContext;
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
                // RandomWordIndex
                Points = makeRoomRequest.Points,
                Capacity = makeRoomRequest.Capacity,
                IsPublic = makeRoomRequest.IsPublic,
                // UsersInRoom
                // Turn
                // DrawerId
                // IsEnd
            };

            _roomData.RoomInfo[room.RoomId] = room;

            var newRoomResponse = new RoomResponse
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
            };

            if (room.IsPublic)
            {
                _ = _hubContext.Clients.All.SendAsync("NewPublicRoomCreated", newRoomResponse);
            }

            return newRoomResponse;
        }

        public RoomResponse GetRoom(string roomId)
        {
            var room = _roomData.RoomInfo.GetValueOrDefault(roomId);
            if (room == null)
            {
                throw new NotFoundException($"Không tìm thấy Room {roomId}");
            }

            if (room.RoomStatus == RoomStatus.Playing || room.RoomStatus == RoomStatus.Finished)
            {
                throw new NotFoundException($"Room đã bắt đầu trò chơi rồi.");
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
            };
        }

        public List<RoomResponse> GetPublicRooms()
        {
            return _roomData.RoomInfo.Values.Where(room => room.IsPublic && room.RoomStatus == RoomStatus.Waiting).Select(room => new RoomResponse
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
            }).ToList();
        }

        public async Task PlayGame(Room room)
        {
            
            await _hubContext.Clients.Group(room.RoomId).SendAsync("StartGame");
            await Task.Delay(3000);
            while (room.RoomStatus == RoomStatus.Playing)
            {
                await StartNewTurn(room.RoomId);
                await Task.Delay(25000);
                 
                var correctAnswer = room.Topic.Words[room.RandomWordIndex];
                await _hubContext.Clients.Group(room.RoomId).SendAsync("ShowAnswer", correctAnswer);
                await Task.Delay(5000);
            }
            await _hubContext.Clients.Group(room.RoomId).SendAsync("EndGame");
        }
        
        private async Task StartNewTurn(string roomId)
        {        
            var room = _roomData.RoomInfo[roomId];
            var userInRooms = room.UsersInRoom;
            room.Turn += 1;
            
            var drawer = userInRooms[(room.Turn - 1) % userInRooms.Count];
            drawer.Turn += 1;
            
            room.DrawerId = drawer.UserId;
            // room.RandomWordIndex = _random.Next(0, room.Topic.Words.Count);
            room.RandomWordIndex = (room.RandomWordIndex + 1) % room.Topic.Words.Count;
                
            await _hubContext.Clients.Group(roomId).SendAsync(
                "StartNewTurn", 
                drawer.UserId, 
                drawer.UserNickName,
                room.Topic.Words[room.RandomWordIndex]
            );
        }
        
    }
}
