using Common;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Client.Hubs
{
    public class ChatHub : Hub
    {
        [Authorize]
        public Task Join(long id)
        {
            return Groups.Add(Context.ConnectionId, id.ToString());
        }

        [Authorize]
        public Task Leave(long id)
        {
            return Groups.Remove(Context.ConnectionId, id.ToString());
        }

        public void SendMessage(long id, ChatMessage message)
        {
            Clients.Group(id.ToString()).newMessage(message);
        }
    }
}