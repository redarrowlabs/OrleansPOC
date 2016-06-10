using Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IProviderGrain : IEntityGrain
    {
        Task Subscribe(INotify notify);

        Task Unsubscribe(INotify notify);

        Task<IEnumerable<Patient>> CurrentPatients();

        Task AddPatient(IPatientGrain patient);
    }
}