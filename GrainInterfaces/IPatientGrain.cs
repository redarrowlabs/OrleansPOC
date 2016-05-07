using Orleans;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IPatientGrain : IGrainWithIntegerKey
    {
        Task<IProviderGrain> CurrentProvider();

        Task SetProvider(IProviderGrain provider);

        Task SendMessage(string message);
    }
}