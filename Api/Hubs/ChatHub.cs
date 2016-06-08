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
        public Task Join(Guid patientId)
        {
            return Groups.Add(Context.ConnectionId, patientId.ToString());
        }

        public Task Leave(Guid patientId)
        {
            return Groups.Remove(Context.ConnectionId, patientId.ToString());
        }

        public async Task SendMessage(Guid patientId, string text)
        {
            var message = new ChatMessage
            {
                Name = Context.User.GetFullName(),
                Received = DateTime.UtcNow,
                Text = text
            };

            var chat = GrainClient.GrainFactory.GetGrain<IChatGrain>(patientId);
            await chat.AddMessage(message);

            Clients.Group(patientId.ToString()).newMessage(message);
        }
    }
}