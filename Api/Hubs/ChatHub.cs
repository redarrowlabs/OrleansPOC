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
        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public async Task Join(Guid patientId)
        {
            var userId = Context.User.GetUserId();
            var groupName = patientId.ToString();

            var chat = GrainClient.GrainFactory.GetGrain<IChatGrain>(patientId);
            var entityName = await chat.Join(Context.User.GetUserId(), GetEntityType());

            await Groups.Add(Context.ConnectionId, groupName);
            await Clients.Group(groupName).joined(new Entity { Id = userId, Name = entityName });
        }

        public async Task Leave(Guid patientId)
        {
            var userId = Context.User.GetUserId();
            var groupName = patientId.ToString();

            var chat = GrainClient.GrainFactory.GetGrain<IChatGrain>(patientId);
            await chat.Leave(userId);

            await Groups.Remove(Context.ConnectionId, groupName);
            await Clients.Group(groupName).left(userId);
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

        private EntityType GetEntityType()
        {
            return Context.User.IsPatient() ? EntityType.Patient : EntityType.Provider;
        }
    }
}