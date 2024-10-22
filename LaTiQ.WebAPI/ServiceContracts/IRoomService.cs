using LaTiQ.Core.DTO.Request.Room;
using LaTiQ.Core.DTO.Response.Room;

namespace LaTiQ.WebAPI.ServiceContracts
{
    public interface IRoomService
    {
        public Task<RoomResponse?> MakeRoom(string email, MakeRoomRequest req);

        public Task<RoomResponse?> GetRoom(string roomId);
    }
}
