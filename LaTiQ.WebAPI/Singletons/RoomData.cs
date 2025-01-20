using LaTiQ.Application.Models;
using LaTiQ.Core.Entities;
using LaTiQ.WebAPI.Constants;

namespace LaTiQ.WebAPI.Singletons
{
    public class RoomData
    {
        public readonly HashSet<int> UniqueNumbers = new();

        // Dictionary<roomId, Room>
        public Dictionary<string, Room> RoomInfo { get; set; } = new Dictionary<string, Room>()
        {
            { "0", new Room
                {
                    RoomId = "0",
                    OwnerId = Guid.Parse("25302ef5-f2fc-4c83-bfd7-ec0b7ef7dc9a"),
                    Topic = TopicData.Topics[2],
                    Points = 50,
                    Capacity = 10,
                    Turn = 0,
                    IsPublic = true,
                    IsLocked = false,
                    UsersInRoom = new List<UserRoom>()
                }
            }
        };

        // Dictionary<Context.ConnectionId(Hub), UserRoom>
        public Dictionary<string, UserRoom> ConnectionUserRoom { get; set; } = new Dictionary<string, UserRoom>();
    }
}
