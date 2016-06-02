using Orleans;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IEntityGrain : IGrainWithGuidKey
    {
        Task<string> GetName();

        Task SetName(string name);
    }
}