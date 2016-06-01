using Common;
using System.Collections.Generic;

namespace Grains.State
{
    public class PatientState
    {
        public PatientState()
        {
            Messages = new List<ChatMessage>();
        }

        public long? ProviderId { get; set; }

        public string Name { get; set; }

        public List<ChatMessage> Messages { get; set; }
    }
}