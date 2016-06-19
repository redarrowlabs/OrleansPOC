using Common;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IChatGrain : IGrainWithGuidKey
    {
        Task<IEnumerable<ChatEntity>> Entities();

        Task<IEnumerable<ChatMessage>> Messages();

        Task<string> Join(Guid entityId, EntityType entityType);

        Task Leave(Guid entityId);

        Task<ChatMessage> AddMessage(Guid entityId, EntityType entityType, string text);

        Task ConfirmMessage(Guid entityId, Guid messageId);

        Task<IEnumerable<ChatMessage>> Search(string searchValue);
    }
}