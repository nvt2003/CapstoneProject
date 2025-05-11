namespace Planify_BackEnd.Hub
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using System.Security.Claims;
    [Authorize]
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            return base.OnConnectedAsync();
        }
        public async Task SendNotification(string message, string link)
        {
            await Clients.All.SendAsync("ReceiveNotification", message, link);
        }
        public async Task SendToUser(string userId, string message, string link)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message, link);
        }
    }
}
