using GrainInterfaces;
using Microsoft.Orleans.ServiceFabric.Client;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using System;

namespace SeedData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = new ClientConfiguration
            {
                DataConnectionString = "UseDevelopmentStorage=true",
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
                StatisticsPerfCountersWriteInterval = TimeSpan.FromDays(6),
            };

            OrleansFabricClient.Initialize(new Uri("fabric:/OrleansPOCFabric/SiloService"), config);

            var pt1 = GrainClient.GrainFactory.GetGrain<IPatientGrain>(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            var pt2 = GrainClient.GrainFactory.GetGrain<IPatientGrain>(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            var pr1 = GrainClient.GrainFactory.GetGrain<IProviderGrain>(Guid.Parse("00000000-0000-0000-0000-000000000003"));
            var pr2 = GrainClient.GrainFactory.GetGrain<IProviderGrain>(Guid.Parse("00000000-0000-0000-0000-000000000004"));

            pt1.SetName("Justin Case").Wait();
            pt2.SetName("Gene Poole").Wait();
            pr1.SetName("Doctor Snuggles").Wait();
            pr2.SetName("Doctor Nobody").Wait();
            pr1.AddPatient(pt1).Wait();
            pr1.AddPatient(pt2).Wait();

            GrainClient.Uninitialize();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}