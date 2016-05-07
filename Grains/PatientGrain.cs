using GrainInterfaces;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class PatientGrain : Grain, IPatientGrain
    {
        private IProviderGrain _provider;

        public Task<IProviderGrain> CurrentProvider()
        {
            return Task.FromResult(_provider);
        }

        public Task SetProvider(IProviderGrain provider)
        {
            _provider = provider;

            return TaskDone.Done;
        }

        public Task SendMessage(string message)
        {
            Console.WriteLine(message);

            return TaskDone.Done;
        }
    }
}