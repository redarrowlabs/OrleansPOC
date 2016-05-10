using Common;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IPatientGrain : IGrainWithIntegerKey
    {
        Task<string> GetName();

        Task SyncProvider(IProviderGrain provider);

        Task<IEnumerable<ChatMessage>> Messages();

        Task AddMessage(ChatMessage message);

        Task SendMessage(string message);
    }
}