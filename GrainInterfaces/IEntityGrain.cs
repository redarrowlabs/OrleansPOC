using Orleans;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IEntityGrain : IGrainWithIntegerKey
    {
        Task<string> GetName();

        Task SetName(string name);
    }
}