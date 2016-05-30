using Common;
using GrainInterfaces;
using Orleans;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    public class PatientGrain : Grain, IPatientGrain
    {
        private string _name;
        private IProviderGrain _provider;
        private List<ChatMessage> _messages;

        public override async Task OnActivateAsync()
        {
            _messages = new List<ChatMessage>();

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
    }
}