using Microsoft.Orleans.ServiceFabric.Client;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Owin;
using System;

public static class OrleansMiddleware
{
    public static void UseOrleans(this IAppBuilder app, string connectionString, string fabricUri)
    {
        app.Use(async (context, next) =>
        {
            if (!GrainClient.IsInitialized)
            {
                var config = new ClientConfiguration
                {
                    DataConnectionString = connectionString,
                    PropagateActivityId = true,
                    DefaultTraceLevel = Severity.Info,
                    GatewayProvider = ClientConfiguration.GatewayProviderType.AzureTable,
                    TraceToConsole = true,
                    TraceFileName = null,
                    TraceFilePattern = null,
                    ResponseTimeout = TimeSpan.FromSeconds(90),
                    StatisticsCollectionLevel = StatisticsLevel.Critical,
                    StatisticsLogWriteInterval = TimeSpan.FromDays(6),
                    StatisticsMetricsTableWriteInterval = TimeSpan.FromDays(6),
                    StatisticsPerfCountersWriteInterval = TimeSpan.FromDays(6)
                };

                OrleansFabricClient.Initialize(new Uri(fabricUri), config);
            }

            await next.Invoke();
        });
    }
}