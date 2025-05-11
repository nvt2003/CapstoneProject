namespace Planify_BackEnd.Hub
{
    using Microsoft.AspNetCore.SignalR;
    using Planify_BackEnd.Models;
    using System.Threading.Tasks;

    public class EventRequestHub : Hub
    {
        public async System.Threading.Tasks.Task SendRequest(SendRequest request)
        {
            await Clients.All.SendAsync("ReceiveEventRequest", request);
        }
    }
}
