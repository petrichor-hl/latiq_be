using LaTiQ.Application.DTOs.Room.Req;
using LaTiQ.Application.DTOs.Room.Res;
using LaTiQ.Core.Entities;

namespace LaTiQ.WebAPI.ServiceContracts
{
    public interface IRoomService
    {
        public Task<RoomResponse> MakeRoom(MakeRoomRequest makeRoomRequest);

        public RoomResponse GetRoom(string roomId);
        
        public Task PlayGame(Room room);
    }
}
