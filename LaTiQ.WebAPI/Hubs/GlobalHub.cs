using System.Security.Claims;
using LaTiQ.WebAPI.ServiceContracts;
using LaTiQ.WebAPI.Singletons;
using Microsoft.AspNetCore.SignalR;
using LaTiQ.Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace LaTiQ.WebAPI.Hubs
{
    [Authorize]
    public class GlobalHub : Hub
    {
        private readonly IUserService _userService;
        private readonly ITopicService _topicService;

        private readonly RoomData _roomData;
        private readonly Random random = new Random();

        public GlobalHub(IUserService userService, ITopicService topicService, RoomData roomData)
        {
            _userService = userService;
            _topicService = topicService;
            _roomData = roomData;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task JoinRoom(UserRoom userRoom)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userRoom.RoomId);
            
            _roomData.UserRooms[Context.ConnectionId] = userRoom;
            var userInRooms = _roomData.UserRooms.Values
                .Where(e => e.RoomId == userRoom.RoomId);
            
            await Clients.Caller.SendAsync("ReceiveUserInRooms", userInRooms);
            await Clients.OthersInGroup(userRoom.RoomId).SendAsync("JoinRoom", userRoom);
        }

        // public async Task ChangeCameraStatus(CameraStatus cameraStatus)
        // {
        //     if (_roomData.UserRooms.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
        //     {
        //         userRoom.CameraStatus = cameraStatus;
        //         await Clients.OthersInGroup(userRoom.RoomId).SendAsync("ChangeCameraStatus", userRoom.UserEmail, cameraStatus);
        //     }
        // }

        public async Task LeaveRoom()
        {
            if (_roomData.UserRooms.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userRoom.RoomId);
                _roomData.UserRooms.Remove(Context.ConnectionId);
                
                var userInRooms = _roomData.UserRooms.Values
                    .Where(e => e.RoomId == userRoom.RoomId).ToList();
                
                if (userInRooms.Any())
                {
                    // Inform to the others in group
                    await Clients.OthersInGroup(userRoom.RoomId).SendAsync("LeaveRoom", userRoom.UserId, userRoom.UserNickName);
                    
                    var room = _roomData.RoomInfo[userRoom.RoomId];
                    if (room.OwnerId == userRoom.UserId)
                    {
                        // Room Owner left => Change room owner to userInRooms[0]
                        room.OwnerId = userInRooms[0].UserId;
                        await Clients.OthersInGroup(room.RoomId).SendAsync("NewRoomOwner", room.OwnerId);
                    }
                }
                else
                {
                    _roomData.RoomInfo[userRoom.RoomId].IsLocked = false;
                    // _roomData.RoomInfo.Remove(userRoom.RoomId);
                }
            }
        }
        
        public async Task StartGame()
        {
            if (_roomData.UserRooms.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                var userId = _roomData.UserRooms[Context.ConnectionId].UserId;
                var room = _roomData.RoomInfo[userRoom.RoomId];
                if (room.OwnerId == userId)
                {
                    room.IsLocked = true;
                    await Clients.Group(room.RoomId).SendAsync("StartGame");
                    await Task.Delay(3000);
                    await SelectDrawer(room.RoomId);
                }
            }
        }

        private async Task SelectDrawer(string roomId)
        {
            var userInRooms = _roomData.UserRooms.Values
                .Where(e => e.RoomId == roomId).OrderBy(e => e.UserId).ToList();
            
            var room = _roomData.RoomInfo[roomId];
            
            var drawer = userInRooms[room.Turn % userInRooms.Count];
            
            room.RandomWordIndex = random.Next(0, room.Topic.Words.Count);
            
            await Clients.Group(roomId).SendAsync(
                "SelectDrawer", 
                drawer.UserId, 
                drawer.UserNickName,
                room.Topic.Words[room.RandomWordIndex]
            );
            
            room.Turn++;
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

        // public async Task Undo()
        // {
        //     if (_roomData.UserRooms.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
        //     {
        //         await Clients.OthersInGroup(userRoom.RoomId).SendAsync("Undo");
        //     }
        // }
        
        public async Task ClearPaint()
        {
            if (_roomData.UserRooms.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                await Clients.OthersInGroup(userRoom.RoomId).SendAsync("ClearPaint");
            }
        }
        #endregion

        #region Answer

        public async Task Answer(string answer, int remainingTime)
        {
            var userRoom = _roomData.UserRooms[Context.ConnectionId];
            var room = _roomData.RoomInfo[userRoom.RoomId];

            if (answer.ToLower() == room.Topic.Words[room.RandomWordIndex].ToLower())
            {
                userRoom.UserPoints += remainingTime;
                if (userRoom.UserPoints >= room.Points)
                {
                    // End Game
                }
                else
                {
                    await Clients.Group(room.RoomId).SendAsync("CorrectAnswer", userRoom.UserId, userRoom.UserNickName, remainingTime);
                }
            }
            else
            {
                await Clients.Group(room.RoomId).SendAsync("IncorrectAnswer", userRoom.UserNickName, answer);
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
