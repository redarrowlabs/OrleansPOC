using Common;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IChatGrain : IGrainWithGuidKey
    {
        Task<IEnumerable<ChatMessage>> Messages();

        Task AddMessage(ChatMessage message);
    }
}