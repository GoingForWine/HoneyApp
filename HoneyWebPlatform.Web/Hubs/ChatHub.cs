using Microsoft.AspNetCore.SignalR;

namespace HoneyWebPlatform.Web.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            //todo "ReceiveMessage" should be inserted as a constant
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
