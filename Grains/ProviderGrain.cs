using Common;
using GrainInterfaces;
using Grains.State;
using Orleans;
using Orleans.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider(ProviderName = "JsonStore")]
    public class ProviderGrain : Grain<ProviderState>, IProviderGrain
    {
        private Dictionary<long, IPatientGrain> _patients;

        public override Task OnActivateAsync()
        {
            _patients = State.Patients
                .Select(x => GrainFactory.GetGrain<IPatientGrain>(x))
                .ToDictionary(x => x.GetPrimaryKeyLong());

            return base.OnActivateAsync();
        }

        public Task<string> GetName()
        {
            return Task.FromResult(State.Name);
        }

        public Task SetName(string name)
        {
            State.Name = name;

            return base.WriteStateAsync();
        }

        public async Task AddPatient(IPatientGrain patient)
        {
            var patientId = patient.GetPrimaryKeyLong();
            if (!_patients.ContainsKey(patientId))
            {
                _patients.Add(patientId, patient);
                await patient.SetProvider(this.GetPrimaryKeyLong());
            }

            State.Patients.Add(patientId);

            await base.WriteStateAsync();
        }

        public async Task<IEnumerable<Patient>> CurrentPatients()
        {
            return await Task.WhenAll(
                _patients.Values
                    .Select(async x =>
                        new Patient
                        {
                            Id = x.GetPrimaryKeyLong(),
                            Name = await x.GetName()
                        }
                    )
            );
        }

        public async Task<IEnumerable<ChatMessage>> Messages(long patientId)
        {
            var messages = await _patients[patientId].Messages();

            return messages.OrderBy(x => x.Received).ToList();
        }
    }
}