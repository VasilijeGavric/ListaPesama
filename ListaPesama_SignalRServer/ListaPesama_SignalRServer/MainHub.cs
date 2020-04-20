using Microsoft.AspNet.SignalR;

namespace ListaPesama_SignalRServer
{
    public class ChatHub : Hub
    {
        public void SendMessage(string message)
        {
            Clients.All.UpdateMessage(message);
        }
    }
}