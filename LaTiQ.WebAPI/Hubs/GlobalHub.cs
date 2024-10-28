using LaTiQ.Core.DTO.Request.Room;
using LaTiQ.Core.DTO.Response.Room;
using LaTiQ.Core.DTO.Response.User;
using LaTiQ.Core.Entities;
using LaTiQ.Core.Entities.Room;
using LaTiQ.WebAPI.ServiceContracts;
using LaTiQ.WebAPI.Services;
using LaTiQ.WebAPI.Singletons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LaTiQ.WebAPI.Hubs
{
    public class GlobalHub : Hub
    {
        private readonly IUserService _userService;
        private readonly ITopicService _topicService;

        private readonly RoomData _roomData;

        public GlobalHub(IUserService userService, ITopicService topicService, RoomData roomData)
        {
            _userService = userService;
            _topicService = topicService;
            _roomData = roomData;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var email = Context.User.FindFirstValue(ClaimTypes.Email);
            Console.WriteLine("Context.ConnectionId = " + Context.ConnectionId);
            Console.WriteLine("email = " + email);
            Console.WriteLine("Context.UserIdentifier = " + Context.UserIdentifier);
        }

        public async Task JoinRoom(UserRoom userRoom)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userRoom.RoomId);
            
            var userInRooms = _roomData.UserRooms.Values
                .Where(e => e.RoomId == userRoom.RoomId);
            
            await Clients.Caller.SendAsync("ReceiveUserInRooms", userInRooms);
            await Clients.OthersInGroup(userRoom.RoomId).SendAsync("JoinRoom", userRoom);
            
            _roomData.UserRooms[Context.ConnectionId] = userRoom;
        }

        public async Task LeaveRoom()
        {
            if (_roomData.UserRooms.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userRoom.RoomId);
                _roomData.UserRooms.Remove(Context.ConnectionId);
                
                var userInRooms = _roomData.UserRooms.Values
                    .Where(e => e.RoomId == userRoom.RoomId);
                
                if (userInRooms.Any())
                {
                    // Inform to the others in group
                    await Clients.Group(userRoom.RoomId).SendAsync("LeaveRoom", userRoom.UserEmail);
                }
                else
                {
                    _roomData.RoomInfo.Remove(userRoom.RoomId);
                }
            }
        }

        #region Drawing
        public async Task BeginPath(string strokeColor, float lineWidth, Point point)
        {
            if (_roomData.UserRooms.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                await Clients.OthersInGroup(userRoom.RoomId).SendAsync("BeginPath", strokeColor, lineWidth, point);
            }
        }

        public async Task LineTo(Point point)
        {
            if (_roomData.UserRooms.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                await Clients.OthersInGroup(userRoom.RoomId).SendAsync("LineTo", point);
            }
        }

        public async Task Undo()
        {
            if (_roomData.UserRooms.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                await Clients.OthersInGroup(userRoom.RoomId).SendAsync("Undo");
            }
        }
        #endregion

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            await LeaveRoom();
        }
    }
}
