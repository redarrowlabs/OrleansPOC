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
    [StorageProvider(ProviderName = "Default")]
    public class ProviderGrain : BaseGrain<ProviderState>, IProviderGrain
    {
        private Dictionary<Guid, IPatientGrain> _patients;

        public override Task OnActivateAsync()
        {
            _patients = State.Patients
                .Select(x => GrainFactory.GetGrain<IPatientGrain>(x))
                .ToDictionary(x => x.GetPrimaryKey());

            RegisterTimer(Notify, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));

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
            var patientId = patient.GetPrimaryKey();
            if (!_patients.ContainsKey(patientId))
            {
                _patients.Add(patientId, patient);
                await patient.SetProvider(this.GetPrimaryKey());

                State.Patients.Add(patientId);
                await base.WriteStateAsync();
            }
        }

        public async Task<IEnumerable<Patient>> CurrentPatients()
        {
            return await Task.WhenAll(
                _patients.Values
                    .Select(async x =>
                        new Patient
                        {
                            Id = x.GetPrimaryKey(),
                            Name = await x.GetName()
                        }
                    )
            );
        }

        private async Task Notify(object state)
        {
            var allChatMessages = await Task.WhenAll(
                _patients.Values
                    .Select(x => GrainFactory.GetGrain<IChatGrain>(x.GetPrimaryKey()))
                    .Select(async x => await x.Messages())
            );

            var notifications = await Task.WhenAll(
                allChatMessages
                    .SelectMany(x => x)
                    .Where(x => !x.Viewed.Contains(this.GetPrimaryKey()))
                    .GroupBy(x => new { x.EntityId, x.EntityType })
                    .Select(async x =>
                    {
                        var entity = GetEntity(x.Key.EntityId, x.Key.EntityType);
                        return new ChatNotification
                        {
                            Name = await entity.GetName(),
                            Count = x.Count()
                        };
                    })
            );

            GetLogger().Info("Provider {0} has {1} notifications", this.GetPrimaryKey(), notifications.Length);

            var streamProvider = GetStreamProvider("Default");
            var stream = streamProvider.GetStream<ChatNotification>(this.GetPrimaryKey(), "ChatNotifications");
            foreach (var n in notifications)
            {
                await stream.OnNextAsync(n);
            }
        }
    }
}