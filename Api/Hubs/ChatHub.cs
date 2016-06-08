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
        public override async Task OnDisconnected(bool stopCalled)
        {
            var userId = Context.User.GetUserId();
            if (Context.User.IsPatient())
            {
                await LeaveChat(userId, userId);
            }

            await base.OnDisconnected(stopCalled);
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
            await LeaveChat(userId, patientId);
            await Groups.Remove(Context.ConnectionId, patientId.ToString());
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

        private async Task LeaveChat(Guid userId, Guid patientId)
        {
            var groupName = patientId.ToString();

            var chat = GrainClient.GrainFactory.GetGrain<IChatGrain>(patientId);
            await chat.Leave(userId);

            await Clients.OthersInGroup(patientId.ToString()).left(userId);
        }

        private EntityType GetEntityType()
        {
            return Context.User.IsPatient() ? EntityType.Patient : EntityType.Provider;
        }
    }
}