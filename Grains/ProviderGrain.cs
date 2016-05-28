using Common;
using GrainInterfaces;
using Microsoft.AspNet.SignalR.Client;
using Orleans;
using System;
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

            _hubConnection = new HubConnection("https://localhost:44302");
            _hub = _hubConnection.CreateHubProxy("ChatHub");
            await _hubConnection.Start();

            await base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            if (_hubConnection != null)
            {
                _hubConnection.Dispose();
            }

            return base.OnDeactivateAsync();
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

        public async Task SendMessage(long patientId, string message)
        {
            var cm = new ChatMessage
            {
                Name = _name,
                Received = DateTime.UtcNow,
                Text = message
            };

            await _patients[patientId].AddMessage(cm);

            if (_hubConnection.State == ConnectionState.Connected)
            {
                await _hub.Invoke("SendMessage", patientId, cm);
            }
        }
    }
}