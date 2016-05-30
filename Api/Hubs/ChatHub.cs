using Api.Infrastructure;
using Common;
using GrainInterfaces;
using Microsoft.AspNet.SignalR;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Client.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public Task Join(long id)
        {
            return Groups.Add(Context.ConnectionId, id.ToString());
        }

        public Task Leave(long id)
        {
            return Groups.Remove(Context.ConnectionId, id.ToString());
        }

        public async Task SendMessage(long id, string text)
        {
            var message = new ChatMessage
            {
                Name = Context.User.GetFullName(),
                Received = DateTime.UtcNow,
                Text = text
            };

            var patient = GrainClient.GrainFactory.GetGrain<IPatientGrain>(id);
            await patient.AddMessage(message);

            Clients.Group(id.ToString()).newMessage(message);
        }
    }
}