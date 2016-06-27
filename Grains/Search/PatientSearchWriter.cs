using Common;
using GrainInterfaces.Search;
using Grains.Infrastructure;
using Orleans;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Grains.Search
{
    public class PatientSearchWriter : Grain, IPatientSearchWriter
    {
        private HashRing<long> _ring;

        public override async Task OnActivateAsync()
        {
            _ring = new HashRing<long>(3, x => x);

            await base.OnActivateAsync();
        }

        public async Task Update(Patient patient)
        {
            var ps = GrainFactory.GetGrain<IPatientSearch>(Guid.Empty);
            await ps.Update(patient);

            var state = await ps.GetState();
            var replicaUpdates = _ring.Nodes.Select(x => GrainFactory.GetGrain<IPatientSearchReadReplica>(x).Sync(state));
            await Task.WhenAll(replicaUpdates);
        }
    }
}