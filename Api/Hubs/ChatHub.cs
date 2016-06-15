using Api.Infrastructure;
using Api.Models;
using Common;
using GrainInterfaces;
using Microsoft.AspNet.SignalR;
using Orleans;
using Orleans.Streams;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Hubs
{
    [AuthorizeUser]
    public class ChatHub : Hub
    {
        public override async Task OnConnected()
        {
            var userId = Context.User.GetUserId();
            if (Context.User.IsProvider())
            {
                Func<ChatNotification, StreamSequenceToken, Task> handleNotification = (notification, token) =>
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    context.Clients.User(userId.ToString()).notifyNewMessage(notification);
                    return TaskDone.Done;
                };

                var streamProvider = GrainClient.GetStreamProvider("Default");
                var stream = streamProvider.GetStream<ChatNotification>(Context.User.GetUserId(), "ChatNotifications");
                var subscriptions = await stream.GetAllSubscriptionHandles();
                if (subscriptions.Any())
                {
                    await Task.WhenAll(subscriptions.Select(x => x.ResumeAsync(handleNotification)));
                }
                else
                {
                    await stream.SubscribeAsync(handleNotification);
                }
            }

            await base.OnConnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            var userId = Context.User.GetUserId();
            if (Context.User.IsPatient())
            {
                await LeaveChat(userId, userId);
            }
            else
            {
                var streamProvider = GrainClient.GetStreamProvider("Default");
                var stream = streamProvider.GetStream<ChatNotification>(Context.User.GetUserId(), "ChatNotifications");
                var subscriptions = await stream.GetAllSubscriptionHandles();
                await Task.WhenAll(subscriptions.Select(x => x.UnsubscribeAsync()));

                var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(userId);
                var patients = await provider.CurrentPatients();
                await Task.WhenAll(patients.Select(x => LeaveChat(userId, x.Id)));
            }

            await base.OnDisconnected(stopCalled);
        }

        public async Task<ChatJoinResponse> Join(Guid patientId)
        {
            var userId = Context.User.GetUserId();
            var groupName = patientId.ToString();

            var chat = GrainClient.GrainFactory.GetGrain<IChatGrain>(patientId);
            var entityName = await chat.Join(Context.User.GetUserId(), GetEntityType());

            await Groups.Add(Context.ConnectionId, groupName);
            await Clients.OthersInGroup(groupName).joined(
                new ChatEntity
                {
                    IsPresent = true,
                    Entity = new Entity
                    {
                        Id = userId,
                        Name = entityName
                    }
                }
            );

            return new ChatJoinResponse
            {
                Messages = await chat.Messages(),
                Users = await chat.Entities()
            };
        }

        public async Task Leave(Guid patientId)
        {
            var userId = Context.User.GetUserId();
            await LeaveChat(userId, patientId);
            await Groups.Remove(Context.ConnectionId, patientId.ToString());
        }

        public async Task SendMessage(Guid patientId, string text)
        {
            var chat = GrainClient.GrainFactory.GetGrain<IChatGrain>(patientId);
            var message = await chat.AddMessage(Context.User.GetUserId(), GetEntityType(), text);

            Clients.Group(patientId.ToString()).newMessage(message);
        }

        public Task ConfirmMessage(Guid patientId, Guid messageId)
        {
            var chat = GrainClient.GrainFactory.GetGrain<IChatGrain>(patientId);
            return chat.ConfirmMessage(Context.User.GetUserId(), messageId);
        }

        private async Task LeaveChat(Guid userId, Guid patientId)
        {
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