using Common;
using System;
using System.Collections.Generic;

namespace Grains.State
{
    public class ChatState
    {
        public ChatState()
        {
            JoinedEntities = new Dictionary<Guid, EntityType>();
            Messages = new List<ChatMessage>();
        }

        public Dictionary<Guid, EntityType> JoinedEntities { get; set; }

        public List<ChatMessage> Messages { get; set; }
    }
}