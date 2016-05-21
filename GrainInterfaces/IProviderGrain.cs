using Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IProviderGrain : IEntityGrain
    {
        Task<IEnumerable<Patient>> CurrentPatients();

        Task AddPatient(IPatientGrain patient);

        Task<IEnumerable<ChatMessage>> Messages(long patientId);

        Task SendMessage(long patientId, string message);
    }
}