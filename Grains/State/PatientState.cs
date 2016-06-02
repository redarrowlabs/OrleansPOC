using Common;
using System;
using System.Collections.Generic;

namespace Grains.State
{
    public class PatientState
    {
        public PatientState()
        {
            Messages = new List<ChatMessage>();
        }

        public Guid? ProviderId { get; set; }

        public string Name { get; set; }

        public List<ChatMessage> Messages { get; set; }
    }
}