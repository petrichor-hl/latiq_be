using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LaTiQ.WebAPI.Hubs
{
    public class GlobalHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var email = Context.User.FindFirstValue(ClaimTypes.Email);
            Console.WriteLine("Context.ConnectionId = " + Context.ConnectionId);
            Console.WriteLine("email = " + email);
            Console.WriteLine("Context.UserIdentifier = " + Context.UserIdentifier);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
