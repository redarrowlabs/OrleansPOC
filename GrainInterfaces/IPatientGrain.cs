using Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IPatientGrain : IEntityGrain
    {
        Task SyncProvider(IProviderGrain provider);

        Task<IEnumerable<ChatMessage>> Messages();

        Task AddMessage(ChatMessage message);
    }
}