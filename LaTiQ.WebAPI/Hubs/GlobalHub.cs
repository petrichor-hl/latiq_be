using LaTiQ.WebAPI.Singletons;
using Microsoft.AspNetCore.SignalR;
using LaTiQ.Application.Models;
using LaTiQ.Core.Entities;
using LaTiQ.WebAPI.ServiceContracts;
using Microsoft.AspNetCore.Authorization;

namespace LaTiQ.WebAPI.Hubs
{
    [Authorize]
    public class GlobalHub : Hub
    {
        private readonly RoomData _roomData;
        
        private readonly IRoomService _roomService;

        public GlobalHub(RoomData roomData, IRoomService roomService)
        {
            _roomData = roomData;
            _roomService = roomService;
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
                
                // 4. Điều chỉnh lại Room.Turn 
                room.Turn -= userRoom.Turn;
                
                // 5. Kiểm tra xem còn bất kỳ ai trong Room không
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
                    Console.WriteLine($"Room {userRoom.RoomId} has no users in room");
                    _roomData.RoomInfo.Remove(userRoom.RoomId);
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
                    await Clients.Group(room.RoomId).SendAsync("StartGame");
                    _ = _roomService.PlayGame(room);
                }
            }
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

        public async Task SendAnswer(string answer, int remainingTime)
        {
            var userRoom = _roomData.ConnectionUserRoom[Context.ConnectionId];
            var room = _roomData.RoomInfo[userRoom.RoomId];

            if (string.Equals(answer, room.Topic.Words[room.RandomWordIndex], StringComparison.CurrentCultureIgnoreCase))
            {
                await Clients.Group(room.RoomId).SendAsync(
                    "AnsweredCorrectly", 
                    userRoom.UserId, 
                    userRoom.UserNickName, 
                    remainingTime
                );

                // Cộng điểm cho Người trả lời đúng
                userRoom.UserPoints += remainingTime;
                if (userRoom.UserPoints >= room.Points)
                {
                    // Đánh dấu ENDGAME
                    room.IsEnd = true;
                }
                
                // Logic tính điểm cho Drawer
                // Không được xoá "room.Turn > 0"
                if (room.Turn > 0)  
                {
                    var userInRooms = room.UsersInRoom;
                    var drawer = userInRooms[(room.Turn - 1) % userInRooms.Count];
                    Console.WriteLine("drawer.UserId  = " + drawer.UserId);
                    Console.WriteLine("room.DrawerId  = " + room.DrawerId);
                    
                    // Cộng điểm cho Drawer
                    if (room.DrawerId == drawer.UserId) 
                    {
                        // Vì sao lại Kiểm tra: room.DrawerId == drawer.UserId ???
                        // Khi người vẽ thoát thì var drawer = userInRooms[(room.Turn - 1) % userInRooms.Count] sẽ không còn đúng nữa
                        // Lúc này room.DrawerId != drawer.UserId
                        drawer.UserPoints += remainingTime;

                        if (drawer.UserPoints >= room.Points)
                        {
                            // Đánh dấu ENDGAME
                            room.IsEnd = true;
                        }
                    }
                }
            }
            else
            {
                await Clients.Group(room.RoomId).SendAsync("AnsweredWrong", userRoom.UserNickName, answer);
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
