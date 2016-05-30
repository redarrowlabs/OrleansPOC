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
                hostConfig.SetServiceName("OrleansPOCiloHost");
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
                var orleansConfig = ClientConfiguration.LocalhostSilo();
                GrainClient.Initialize(orleansConfig);

                var pt1 = GrainClient.GrainFactory.GetGrain<IPatientGrain>(1);
                var pt2 = GrainClient.GrainFactory.GetGrain<IPatientGrain>(2);
                var pr1 = GrainClient.GrainFactory.GetGrain<IProviderGrain>(1);

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