using GrainInterfaces;
using Orleans;
using Orleans.Runtime.Configuration;
using System;
using Topshelf;

namespace Server
{
    public class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(hostConfig =>
            {
                hostConfig.RunAsPrompt();
                hostConfig.StartAutomatically();
                hostConfig.SetServiceName("OrleansPOCSiloHost");
                hostConfig.SetDisplayName("Orleans POC Silo Host");

                hostConfig.Service<OrleansService>(serviceConfig =>
                {
                    serviceConfig.ConstructUsing(settings => new OrleansService());
                    serviceConfig.WhenStarted((service, control) => service.Start());
                    serviceConfig.WhenStopped((service, control) => service.Stop());
                    serviceConfig.WhenShutdown((service, control) => service.Stop());
                    serviceConfig.AfterStartingService(context => SeedData());
                });
            });
        }

        private static void SeedData()
        {
            var sdd = AppDomain.CreateDomain("SeedData");
            sdd.DoCallBack(() =>
            {
                var orleansConfig = ClientConfiguration.StandardLoad();
                GrainClient.Initialize(orleansConfig);

                var pt1 = GrainClient.GrainFactory.GetGrain<IPatientGrain>(Guid.Parse("00000000-0000-0000-0000-000000000001"));
                var pt2 = GrainClient.GrainFactory.GetGrain<IPatientGrain>(Guid.Parse("00000000-0000-0000-0000-000000000002"));
                var pr1 = GrainClient.GrainFactory.GetGrain<IProviderGrain>(Guid.Parse("00000000-0000-0000-0000-000000000003"));

                pt1.SetName("Justin Case").Wait();
                pt2.SetName("Gene Poole").Wait();
                pr1.SetName("Doctor Snuggles").Wait();
                pr1.AddPatient(pt1).Wait();
                pr1.AddPatient(pt2).Wait();

                GrainClient.Uninitialize();
            });

            AppDomain.Unload(sdd);
        }
    }
}