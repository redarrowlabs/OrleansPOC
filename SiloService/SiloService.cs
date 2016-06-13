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
                new OrleansCommunicationListener(context, GetClusterConfiguration(context), Partition)
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

        public ClusterConfiguration GetClusterConfiguration(StatelessServiceContext context)
        {
            var config = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var clusterConfig = new ClusterConfiguration();

            clusterConfig.Defaults.TraceFileName = null;
            clusterConfig.Defaults.TraceFilePattern = null;
            clusterConfig.Defaults.StatisticsCollectionLevel = StatisticsLevel.Info;
            clusterConfig.Defaults.StatisticsLogWriteInterval = TimeSpan.FromDays(6);
            clusterConfig.Defaults.TurnWarningLengthThreshold = TimeSpan.FromSeconds(15);
            clusterConfig.Defaults.TraceToConsole = true;
            clusterConfig.Defaults.DefaultTraceLevel = Severity.Info;

            clusterConfig.Globals.ResponseTimeout = TimeSpan.FromSeconds(90);
            clusterConfig.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.AzureTable;
            clusterConfig.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.AzureTable;
            clusterConfig.Globals.DataConnectionString = config.Settings.Sections["Orleans"].Parameters["SystemStoreConnectionString"].Value;

            var storageOptions = new Dictionary<string, string>
            {
                { "DataConnectionString", config.Settings.Sections["Orleans"].Parameters["StorageProviderConnectionString"].Value }
            };

            clusterConfig.Globals.RegisterStorageProvider<AzureBlobStorage>("Default", storageOptions);

            return clusterConfig;
        }
    }
}