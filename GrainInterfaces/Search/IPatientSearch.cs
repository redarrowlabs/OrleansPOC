using Common;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces.Search
{
    public interface IPatientSearch : IGrainWithGuidKey
    {
        Task Update(Patient patient);

        Task<IEnumerable<Patient>> GetState();
    }
}