namespace LaTiQ.WebAPI.Singletons;

public class UserConnection
{
    // Dictionary<UserId, ConnectionId>
    public readonly Dictionary<Guid, string> Mapping = new ();
}