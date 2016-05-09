using Common;
using GrainInterfaces;
using Microsoft.AspNet.SignalR.Client;
using Orleans;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    public class PatientGrain : Grain, IPatientGrain
    {
        private HubConnection _hubConnection;
        private IHubProxy _hub;

        private IProviderGrain _provider;
        private List<ChatMessage> _messages;

        public override async Task OnActivateAsync()
        {
            _messages = new List<ChatMessage>();

            _hubConnection = new HubConnection("http://localhost:8080");
            _hub = _hubConnection.CreateHubProxy("ChatHub");
            await _hubConnection.Start();

            await base.OnActivateAsync();
        }

        public Task SyncProvider(IProviderGrain provider)
        {
            _provider = provider;

            return TaskDone.Done;
        }

        public Task<IEnumerable<ChatMessage>> Messages()
        {
            return Task.FromResult(_messages.AsEnumerable());
        }

        public Task AddMessage(ChatMessage message)
        {
            _messages.Add(message);

            return TaskDone.Done;
        }

        public Task SendMessage(ChatMessage message)
        {
            _messages.Add(message);
            if (_hubConnection.State == ConnectionState.Connected)
            {
                _hub.Invoke("SendMessage", this.GetPrimaryKeyLong(), message);
            }

            return TaskDone.Done;
        }
    }
}