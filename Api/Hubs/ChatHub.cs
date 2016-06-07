using Api.Infrastructure;
using Common;
using GrainInterfaces;
using Microsoft.AspNet.SignalR;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Client.Hubs
{
    [AuthorizeUser]
    public class ChatHub : Hub
    {
        public Task Join(Guid groupId)
        {
            return Groups.Add(Context.ConnectionId, groupId.ToString());
        }

        public Task Leave(Guid groupId)
        {
            return Groups.Remove(Context.ConnectionId, groupId.ToString());
        }

        public async Task SendMessage(Guid groupId, string text)
        {
            var message = new ChatMessage
            {
                Name = Context.User.GetFullName(),
                Received = DateTime.UtcNow,
                Text = text
            };

            var patient = GrainClient.GrainFactory.GetGrain<IPatientGrain>(groupId);
            await patient.AddMessage(message);

            Clients.Group(groupId.ToString()).newMessage(message);
        }
    }
}