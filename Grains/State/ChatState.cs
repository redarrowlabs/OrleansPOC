using Common;
using System;
using System.Collections.Generic;

namespace Grains.State
{
    public class ChatState
    {
        public ChatState()
        {
            Present = new Dictionary<Guid, EntityType>();
            Messages = new Dictionary<Guid, ChatMessage>();
        }

        public Dictionary<Guid, EntityType> Present { get; set; }

        public Dictionary<Guid, ChatMessage> Messages { get; set; }
    }
}