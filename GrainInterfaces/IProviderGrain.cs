using Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IProviderGrain : IEntityGrain
    {
        Task<IEnumerable<Patient>> CurrentPatients();

        Task AddPatient(IPatientGrain patient);
    }
}