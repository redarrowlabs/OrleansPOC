using Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IPatientGrain : IEntityGrain
    {
        Task SetProvider(long providerId);

        Task<IEnumerable<ChatMessage>> Messages();

        Task AddMessage(ChatMessage message);
    }
}