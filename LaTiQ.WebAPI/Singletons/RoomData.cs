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
                    // RandomWordIndex
                    Points = 50,
                    Capacity = 10,
                    // UsersInRoom
                    // Turn
                    // DrawerId
                    // IsEnd
                }
            },
            { "1", new Room
                {
                    RoomId = "1",
                    OwnerId = Guid.Parse("25302ef5-f2fc-4c83-bfd7-ec0b7ef7dc9a"),
                    Topic = TopicData.Topics[3],
                    // RandomWordIndex
                    Points = 80,
                    Capacity = 10,
                    // UsersInRoom
                    // Turn
                    // DrawerId
                    // IsEnd
                }
            },
            { "2", new Room
                {
                    RoomId = "2",
                    OwnerId = Guid.Parse("25302ef5-f2fc-4c83-bfd7-ec0b7ef7dc9a"),
                    Topic = TopicData.Topics[4],
                    // RandomWordIndex
                    Points = 120,
                    Capacity = 10,
                    // UsersInRoom
                    // Turn
                    // DrawerId
                    // IsEnd
                }
            }
        };

        // Dictionary<Context.ConnectionId(Hub), UserRoom>
        public Dictionary<string, UserRoom> ConnectionUserRoom { get; set; } = new Dictionary<string, UserRoom>();
    }
}
