using GrainInterfaces;
using Grains.State;
using Orleans.Providers;
using System;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider(ProviderName = "Default")]
    public class PatientGrain : BaseGrain<PatientState>, IPatientGrain
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
    }
}