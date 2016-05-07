using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;
using System.Collections.Generic;

namespace Grains
{
    public class ProviderGrain : Grain, IProviderGrain
    {
        private List<IPatientGrain> _patients;


    }
}