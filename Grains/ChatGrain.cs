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
    [StorageProvider(ProviderName = "JsonStore")]
    public class ChatGrain : Grain<ChatState>, IChatGrain
    {
        private Dictionary<Guid, IEntityGrain> _joinedEntities;

        public override Task OnActivateAsync()
        {
            _joinedEntities = State.JoinedEntities
                .Select(x => GetEntity(x.Key, x.Value))
                .ToDictionary(x => x.GetPrimaryKey());

            return base.OnActivateAsync();
        }

        public Task<IEnumerable<ChatMessage>> Messages()
        {
            return Task.FromResult(State.Messages.AsEnumerable());
        }

        public async Task<IEnumerable<Entity>> Users()
        {
            return await Task.WhenAll(
                _joinedEntities.Values
                    .Select(async x =>
                        new Entity
                        {
                            Id = x.GetPrimaryKey(),
                            Name = await x.GetName()
                        }
                    )
            );
        }

        public Task AddMessage(ChatMessage message)
        {
            State.Messages.Add(message);

            return base.WriteStateAsync();
        }

        public async Task<string> Join(Guid entityId, EntityType entityType)
        {
            var entity = GetEntity(entityId, entityType);
            if (!_joinedEntities.ContainsKey(entityId))
            {
                _joinedEntities.Add(entityId, entity);
                State.JoinedEntities.Add(entityId, entityType);
                await base.WriteStateAsync();
            }

            return await entity.GetName();
        }

        public async Task Leave(Guid entityId)
        {
            State.JoinedEntities.Remove(entityId);
            await base.WriteStateAsync();
        }

        private IEntityGrain GetEntity(Guid entityId, EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Patient:
                    return GrainFactory.GetGrain<IPatientGrain>(entityId);

                case EntityType.Provider:
                    return GrainFactory.GetGrain<IProviderGrain>(entityId);

                default:
                    return null;
            }
        }
    }
}