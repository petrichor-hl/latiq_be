﻿using System.Security.Claims;
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
        private readonly UserConnection _userConnection;
        
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        
        public GlobalHub(RoomData roomData, IRoomService roomService, IUserService userService, UserConnection userConnection)
        {
            _roomData = roomData;
            _roomService = roomService;
            _userService = userService;
            _userConnection = userConnection;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            
            // Thêm KV value: <userId, Context.ConnectionId>
            var userId = Guid.Parse(Context.User.FindFirstValue("UserId"));
            _userConnection.Mapping.Add(userId, Context.ConnectionId);
            
            // Cập nhật trạng thái User là Online
            await _userService.UpdateStatus(true);
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // The ConnectionId still exists here
            // Console.WriteLine("OnDisconnectedAsync " + Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
            
            // Xoá KV value: <userId, Context.ConnectionId>
            var userId = Guid.Parse(Context.User.FindFirstValue("UserId"));
            _userConnection.Mapping.Remove(userId);

            // Cập nhật trạng thái User là Offline
            await _userService.UpdateStatus(false);
            
            await LeaveRoom();
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
                        try
                        {
                            room.OwnerId = room.UsersInRoom[0].UserId;
                            await Clients.OthersInGroup(userRoom.RoomId).SendAsync("NewRoomOwner", room.OwnerId);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Room {userRoom.RoomId} has no users in room");
                    _ = Clients.All.SendAsync("DeletePublicRoom", room.RoomId);
                    // Optional:
                    // Nếu trò chơi chưa kết thúc, nhưng trong room không còn ai
                    // => Đánh dấu IsEnd = True để kết thúc vòng While
                    _roomData.RoomInfo[userRoom.RoomId].RoomStatus = RoomStatus.Finished;
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
                    room.RoomStatus = RoomStatus.Playing;
                     _ = Clients.Group(room.RoomId).SendAsync("StartGame");
                     _ = Clients.All.SendAsync("DeletePublicRoom", room.RoomId);
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
                    room.RoomStatus = RoomStatus.Finished;
                }
                
                // Logic tính điểm cho Drawer
                // Không được xoá "room.Turn > 0"
                if (room.Turn > 0)  
                {
                    var userInRooms = room.UsersInRoom;
                    var drawer = userInRooms[(room.Turn - 1) % userInRooms.Count];
                    
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
                            room.RoomStatus = RoomStatus.Finished;
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

        public async Task SendMessage(string message)
        {
            var userRoom = _roomData.ConnectionUserRoom[Context.ConnectionId];
            var room = _roomData.RoomInfo[userRoom.RoomId];

            await Clients.Group(room.RoomId).SendAsync("AnsweredWrong", userRoom.UserNickName, message);
        }
        

    }
}
