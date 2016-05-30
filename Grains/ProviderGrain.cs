using Common;
using GrainInterfaces;
using Microsoft.AspNet.SignalR.Client;
using Orleans;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    public class ProviderGrain : Grain, IProviderGrain
    {
        private HubConnection _hubConnection;
        private IHubProxy _hub;

        private string _name;
        private Dictionary<long, IPatientGrain> _patients;

        public override async Task OnActivateAsync()
        {
            _patients = new Dictionary<long, IPatientGrain>();

            await base.OnActivateAsync();
        }

        public Task<string> GetName()
        {
            return Task.FromResult(_name);
        }

        public Task SetName(string name)
        {
            _name = name;

            return TaskDone.Done;
        }

        public async Task AddPatient(IPatientGrain patient)
        {
            if (!_patients.ContainsKey(patient.GetPrimaryKeyLong()))
            {
                _patients.Add(patient.GetPrimaryKeyLong(), patient);
                await patient.SyncProvider(this);
            }
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