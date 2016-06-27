using Common;
using Orleans;
using System.Threading.Tasks;

namespace GrainInterfaces.Search
{
    public interface IPatientSearchWriter : IGrainWithGuidKey
    {
        Task Update(Patient patient);
    }
}