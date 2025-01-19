using LaTiQ.Application.DTOs.Room.Req;
using LaTiQ.Application.DTOs.Room.Res;

namespace LaTiQ.WebAPI.ServiceContracts
{
    public interface IRoomService
    {
        public Task<RoomResponse> MakeRoom(MakeRoomRequest makeRoomRequest);

        public RoomResponse GetRoom(string roomId);
    }
}
