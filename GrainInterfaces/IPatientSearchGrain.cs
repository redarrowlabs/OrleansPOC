using Common;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IPatientSearchGrain : IGrainWithGuidKey
    {
        Task<IEnumerable<Patient>> Search(string searchValue);
    }
}