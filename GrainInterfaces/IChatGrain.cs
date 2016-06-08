using Common;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IChatGrain : IGrainWithGuidKey
    {
        Task<IEnumerable<Entity>> Users();

        Task<IEnumerable<ChatMessage>> Messages();

        Task AddMessage(ChatMessage message);

        Task<string> Join(Guid entityId, EntityType entityType);

        Task Leave(Guid entityId);
    }
}