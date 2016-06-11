using Microsoft.Orleans.ServiceFabric.Silo;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace SiloService
{
    internal sealed class SiloService : StatelessService
    {
        public SiloService(StatelessServiceContext context)
            : base(context) { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            var silo = new ServiceInstanceListener(context =>
                new OrleansCommunicationListener(context, GetClusterConfiguration(), Partition)
            );

            return new[] { silo };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        public ClusterConfiguration GetClusterConfiguration()
        {
            var config = new ClusterConfiguration();

            config.Defaults.TraceFileName = null;
            config.Defaults.TraceFilePattern = null;
            config.Defaults.StatisticsCollectionLevel = StatisticsLevel.Info;
            config.Defaults.StatisticsLogWriteInterval = TimeSpan.FromDays(6);
            config.Defaults.TurnWarningLengthThreshold = TimeSpan.FromSeconds(15);
            config.Defaults.TraceToConsole = true;
            config.Defaults.DefaultTraceLevel = Severity.Info;

            config.Globals.ResponseTimeout = TimeSpan.FromSeconds(90);
            config.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.AzureTable;
            config.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.AzureTable;

            var storageOptions = new Dictionary<string, string>
            {
                { "DataConnectionString", "UseDevelopmentStorage=true" }
            };

            config.Globals.RegisterStorageProvider<AzureBlobStorage>("Default", storageOptions);

            return config;
        }
    }
}