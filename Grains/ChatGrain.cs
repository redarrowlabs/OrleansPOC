using Common;
using GrainInterfaces;
using Grains.State;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider(ProviderName = "Default")]
    public class ChatGrain : BaseGrain<ChatState>, IChatGrain
    {
        public async Task<IEnumerable<ChatEntity>> Entities()
        {
            return await Task.WhenAll(
                State.Messages.Values
                    .Select(x => new { Id = x.EntityId, Type = x.EntityType })
                    .Union(State.Present.Select(x => new { Id = x.Key, Type = x.Value }))
                    .Distinct()
                    .Select(async x =>
                    {
                        var entity = GetEntity(x.Id, x.Type);
                        return new ChatEntity
                        {
                            IsPresent = State.Present.ContainsKey(x.Id),
                            Entity = new Entity
                            {
                                Id = entity.GetPrimaryKey(),
                                Name = await entity.GetName()
                            }
                        };
                    })
            );
        }

        public Task<IEnumerable<ChatMessage>> Messages()
        {
            var messages = State.Messages.Values
                .OrderByDescending(x => x.Received)
                .Reverse()
                .ToList()
                .AsEnumerable();

            return Task.FromResult(messages);
        }

        public async Task<string> Join(Guid entityId, EntityType entityType)
        {
            var entity = GetEntity(entityId, entityType);
            if (!State.Present.ContainsKey(entityId))
            {
                State.Present.Add(entityId, entityType);
                await base.WriteStateAsync();
            }

            return await entity.GetName();
        }

        public async Task Leave(Guid entityId)
        {
            State.Present.Remove(entityId);
            await base.WriteStateAsync();
        }

        public async Task<ChatMessage> AddMessage(Guid entityId, EntityType entityType, string text)
        {
            var entity = GetEntity(entityId, entityType);
            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                EntityId = entityId,
                EntityType = entityType,
                Name = await entity.GetName(),
                Received = DateTime.UtcNow,
                Text = text
            };

            State.Messages.Add(message.Id, message);
            await base.WriteStateAsync();

            return message;
        }

        public Task ConfirmMessage(Guid entityId, Guid messageId)
        {
            var message = State.Messages[messageId];
            if (!message.Viewed.Contains(entityId))
            {
                message.Viewed.Add(entityId);
            }

            return base.WriteStateAsync();
        }
    }
}