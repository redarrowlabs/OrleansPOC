using Common;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IProviderGrain : IGrainWithIntegerKey
    {
        Task<IEnumerable<IPatientGrain>> CurrentPatients();

        Task AddPatient(IPatientGrain patient);

        Task<IEnumerable<ChatMessage>> Messages(long patientId);

        Task SendMessage(long patientId, ChatMessage message);
    }
}