using System;
using System.Collections.Generic;

namespace Grains.State
{
    public class ProviderState
    {
        public ProviderState()
        {
            Patients = new List<Guid>();
        }

        public string Name { get; set; }

        public List<Guid> Patients { get; set; }
    }
}