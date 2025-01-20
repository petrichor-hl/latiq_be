using LaTiQ.WebAPI.Singletons;
using Microsoft.AspNetCore.SignalR;
using LaTiQ.Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace LaTiQ.WebAPI.Hubs
{
    [Authorize]
    public class GlobalHub : Hub
    {
        private readonly RoomData _roomData;
        private readonly Random _random = new Random();

        public GlobalHub(RoomData roomData)
        {
            _roomData = roomData;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task JoinRoom(UserRoom userRoom)
        {
            // 1. Thêm ConnectionId vào Group RoomId
            await Groups.AddToGroupAsync(Context.ConnectionId, userRoom.RoomId);
            
            // 2. Lưu KV pair: <ConnectionId, UserRoom>
            _roomData.ConnectionUserRoom[Context.ConnectionId] = userRoom;
            
            // 3. Thêm UserRoom vào Room.UsersInRoom
            var room = _roomData.RoomInfo[userRoom.RoomId];
            room.UsersInRoom.Add(userRoom);
            
            await Clients.Caller.SendAsync("ReceiveUserInRooms", room.UsersInRoom);
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
            if (_roomData.ConnectionUserRoom.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                // 1. Xoá ConnectionId khỏi Group RoomId
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userRoom.RoomId);
                
                // 2. Xoá KV pair: <ConnectionId, UserRoom>
                _roomData.ConnectionUserRoom.Remove(Context.ConnectionId);
                
                // 3. Xoá UserRoom khỏi Room.UsersInRoom
                var room = _roomData.RoomInfo[userRoom.RoomId];
                room.UsersInRoom.Remove(userRoom);
                
                if (room.UsersInRoom.Any())
                {
                    // Inform to the others in group
                    await Clients.OthersInGroup(userRoom.RoomId).SendAsync("LeaveRoom", userRoom.UserId, userRoom.UserNickName);
                    
                    if (room.OwnerId == userRoom.UserId)
                    {
                        // Room Owner left => Change room owner to userInRooms[0]
                        room.OwnerId = room.UsersInRoom[0].UserId;
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
            if (_roomData.ConnectionUserRoom.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                var userId = _roomData.ConnectionUserRoom[Context.ConnectionId].UserId;
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
            var room = _roomData.RoomInfo[roomId];
            var userInRooms = room.UsersInRoom;
            
            var drawer = userInRooms[room.Turn % userInRooms.Count];
            
            room.RandomWordIndex = _random.Next(0, room.Topic.Words.Count);
            
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
            if (_roomData.ConnectionUserRoom.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                await Clients.OthersInGroup(userRoom.RoomId).SendAsync("BeginPath", strokeColor, lineWidth, point);
            }
        }

        public async Task LineTo(Point point)
        {
            if (_roomData.ConnectionUserRoom.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                await Clients.OthersInGroup(userRoom.RoomId).SendAsync("LineTo", point);
            }
        }
        
        public async Task ClearPaint()
        {
            if (_roomData.ConnectionUserRoom.TryGetValue(Context.ConnectionId, out UserRoom? userRoom))
            {
                await Clients.OthersInGroup(userRoom.RoomId).SendAsync("ClearPaint");
            }
        }
        #endregion

        #region Answer

        public async Task Answer(string answer, int remainingTime)
        {
            var userRoom = _roomData.ConnectionUserRoom[Context.ConnectionId];
            var room = _roomData.RoomInfo[userRoom.RoomId];

            if (string.Equals(answer, room.Topic.Words[room.RandomWordIndex], StringComparison.CurrentCultureIgnoreCase))
            {
                var userInRooms = room.UsersInRoom;
                var drawer = userInRooms[room.Turn % userInRooms.Count];
                // Cộng điểm cho Người vẽ
                drawer.UserPoints += remainingTime;
                
                // Cộng điểm cho Người trả lời đúng
                userRoom.UserPoints += remainingTime;

                if (drawer.UserPoints >= room.Points || userRoom.UserPoints >= room.Points)
                {
                    // ENDGAME
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
            // The ConnectionId still exists here
            // Console.WriteLine("OnDisconnectedAsync " + Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
            await LeaveRoom();
        }
    }
}
