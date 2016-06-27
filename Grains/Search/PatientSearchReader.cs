using Common;
using GrainInterfaces.Search;
using Grains.Infrastructure;
using Orleans;
using Orleans.Concurrency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains.Search
{
    [StatelessWorker]
    public class PatientSearchReader : Grain, IPatientSearchReader
    {
        private HashRing<long> _ring;

        public override Task OnActivateAsync()
        {
            _ring = new HashRing<long>(3, x => x);

            return base.OnActivateAsync();
        }

        public Task<IEnumerable<Patient>> Search(string sessionId, string searchValue)
        {
            var replicaId = _ring.GetNode(sessionId);
            GetLogger().Info("Performing patient search using replica {0}", replicaId);

            var replica = GrainFactory.GetGrain<IPatientSearchReadReplica>(replicaId);
            return replica.Search(searchValue);
        }
    }
}