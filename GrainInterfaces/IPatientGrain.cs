using System;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IPatientGrain : IEntityGrain
    {
        Task SetProvider(Guid providerId);
    }
}