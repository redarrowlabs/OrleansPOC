using System;

namespace Grains.State
{
    public class PatientState
    {
        public Guid? ProviderId { get; set; }

        public string Name { get; set; }
    }
}