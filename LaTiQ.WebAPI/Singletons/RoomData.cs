using LaTiQ.Core.DTO.Request.Room;
using LaTiQ.Core.Entities.Room;

namespace LaTiQ.WebAPI.Singletons
{
    public class RoomData
    {
        public readonly HashSet<int> _uniqueNumbers = new();

        // Dictionary<roomId, Room>
        public Dictionary<string, Room> RoomInfo { get; set; } = new Dictionary<string, Room>();

        // Dictionary<Context.ConnectionId(Hub), UserRoom>
        public Dictionary<string, UserRoom> UserRooms { get; set; } = new Dictionary<string, UserRoom>();
    }
}
