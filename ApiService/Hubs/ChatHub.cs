using ApiService.Infrastructure;
using ApiService.Models;
using Common;
using GrainInterfaces;
using Microsoft.AspNet.SignalR;
using Orleans;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Hubs
{
    [AuthorizeUser]
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<Guid, INotify> _providerNotifiers;

        static ChatHub()
        {
            _providerNotifiers = new ConcurrentDictionary<Guid, INotify>();
        }

        public override async Task OnConnected()
        {
            var userId = Context.User.GetUserId();
            if (Context.User.IsProvider())
            {
                if (!_providerNotifiers.ContainsKey(userId))
                {
                    var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(userId);
                    var notify = await GrainClient.GrainFactory.CreateObjectReference<INotify>(new ProviderNotify(userId));
                    _providerNotifiers.TryAdd(userId, notify);

                    await provider.Subscribe(notify);
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
                var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(userId);
                await provider.Unsubscribe(_providerNotifiers[userId]);

                INotify _;
                _providerNotifiers.TryRemove(userId, out _);

                var patients = await provider.CurrentPatients();
                var leaveTasks = patients.Select(x => LeaveChat(userId, x.Id)).ToArray();
                await Task.WhenAll(leaveTasks);
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