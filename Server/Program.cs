using GrainInterfaces;
using Orleans;
using Orleans.Runtime.Configuration;
using System;

namespace Server
{
    public class Program
    {
        private static OrleansHostWrapper hostWrapper;

        private static void Main(string[] args)
        {
            AppDomain hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
            {
                AppDomainInitializer = InitSilo,
                AppDomainInitializerArguments = args,
            });

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

            Console.WriteLine("Orleans Silo is running.\nPress Enter to terminate...");
            Console.ReadKey(true);

            hostDomain.DoCallBack(ShutdownSilo);
        }

        private static void InitSilo(string[] args)
        {
            hostWrapper = new OrleansHostWrapper(args);

            if (!hostWrapper.Run())
            {
                Console.Error.WriteLine("Failed to initialize Orleans silo");
            }
        }

        private static void ShutdownSilo()
        {
            if (hostWrapper != null)
            {
                hostWrapper.Dispose();
                GC.SuppressFinalize(hostWrapper);
            }
        }
    }
}