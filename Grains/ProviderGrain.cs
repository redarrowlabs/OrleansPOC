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

        private Dictionary<long, IPatientGrain> _patients;

        public override async Task OnActivateAsync()
        {
            _patients = new Dictionary<long, IPatientGrain>();

            _hubConnection = new HubConnection("http://localhost:8080");
            _hub = _hubConnection.CreateHubProxy("ChatHub");
            await _hubConnection.Start();

            await base.OnActivateAsync();
        }

        public async Task AddPatient(IPatientGrain patient)
        {
            _patients.Add(patient.GetPrimaryKeyLong(), patient);
            await patient.SyncProvider(this);
        }

        public Task<IEnumerable<IPatientGrain>> CurrentPatients()
        {
            return Task.FromResult(_patients.Values.AsEnumerable());
        }

        public async Task<IEnumerable<ChatMessage>> Messages(long patientId)
        {
            var messages = await _patients[patientId].Messages();

            return messages.OrderBy(x => x.Received).ToList();
        }

        public Task SendMessage(long patientId, ChatMessage message)
        {
            _patients[patientId].AddMessage(message);

            if (_hubConnection.State == ConnectionState.Connected)
            {
                _hub.Invoke("SendMessage", patientId, message);
            }

            return TaskDone.Done;
        }
    }
}