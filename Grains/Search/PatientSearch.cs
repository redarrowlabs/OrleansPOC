using Common;
using GrainInterfaces.Search;
using Grains.State;
using Orleans;
using Orleans.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains.Search
{
    [StorageProvider(ProviderName = "Default")]
    public class PatientSearch : Grain<PatientSearchState>, IPatientSearch
    {
        public Task<IEnumerable<Patient>> GetState()
        {
            return Task.FromResult(State.Patients.Values.AsEnumerable());
        }

        public Task Update(Patient patient)
        {
            State.Patients[patient.Id] = patient;

            return base.WriteStateAsync();
        }
    }
}