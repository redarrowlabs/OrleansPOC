using Common;
using GrainInterfaces;
using Grains.State;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider(ProviderName = "JsonStore")]
    public class PatientGrain : Grain<PatientState>, IPatientGrain
    {
        private IProviderGrain _provider;

        public override Task OnActivateAsync()
        {
            if (State.ProviderId.HasValue)
            {
                _provider = GrainFactory.GetGrain<IProviderGrain>(State.ProviderId.Value);
            }

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

        public Task SetProvider(Guid providerId)
        {
            State.ProviderId = providerId;
            _provider = GrainFactory.GetGrain<IProviderGrain>(State.ProviderId.Value);

            return base.WriteStateAsync();
        }

        public Task<IEnumerable<ChatMessage>> Messages()
        {
            return Task.FromResult(State.Messages.AsEnumerable());
        }

        public Task AddMessage(ChatMessage message)
        {
            State.Messages.Add(message);

            return base.WriteStateAsync();
        }
    }
}