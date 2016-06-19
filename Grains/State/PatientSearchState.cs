using Common;
using System;
using System.Collections.Generic;

namespace Grains.State
{
    public class PatientSearchState
    {
        public PatientSearchState()
        {
            Patients = new Dictionary<Guid, Patient>();
        }

        public Dictionary<Guid, Patient> Patients { get; set; }
    }
}