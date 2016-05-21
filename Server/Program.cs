using GrainInterfaces;
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

            // Seed Data
            var orleansConfig = Orleans.Runtime.Configuration.ClientConfiguration.LocalhostSilo();
            Orleans.GrainClient.Initialize(orleansConfig);

            var pt1 = Orleans.GrainClient.GrainFactory.GetGrain<IPatientGrain>(1);
            pt1.SetName("Justin Case");

            var pt2 = Orleans.GrainClient.GrainFactory.GetGrain<IPatientGrain>(2);
            pt2.SetName("Gene Poole");

            var pr1 = Orleans.GrainClient.GrainFactory.GetGrain<IProviderGrain>(1);
            pr1.SetName("Dr. Snuggles");

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