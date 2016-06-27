using Common;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces.Search
{
    public interface IPatientSearchReader : IGrainWithGuidKey
    {
        Task<IEnumerable<Patient>> Search(string sessionId, string searchValue);
    }
}