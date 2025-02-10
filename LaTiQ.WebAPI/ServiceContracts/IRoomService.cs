using LaTiQ.Application.DTOs.Room.Req;
using LaTiQ.Application.DTOs.Room.Res;
using LaTiQ.Core.Entities;

namespace LaTiQ.WebAPI.ServiceContracts
{
    public interface IRoomService
    {
        public Task<RoomResponse> MakeRoom(MakeRoomRequest makeRoomRequest);

        public RoomResponse GetRoom(string roomId);
        
        public List<RoomResponse> GetPublicRooms();
        
        public Task PlayGame(Room room);
    }
}
