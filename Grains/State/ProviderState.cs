using System.Collections.Generic;

namespace Grains.State
{
    public class ProviderState
    {
        public ProviderState()
        {
            Patients = new List<long>();
        }

        public string Name { get; set; }

        public List<long> Patients { get; set; }
    }
}