using Common;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces.Search
{
    public interface IPatientSearchReadReplica : IGrainWithIntegerKey
    {
        Task Sync(IEnumerable<Patient> patients);

        Task<IEnumerable<Patient>> Search(string searchValue);
    }
}