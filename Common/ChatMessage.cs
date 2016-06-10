using System;
using System.Collections.Generic;

namespace Common
{
    public class ChatMessage
    {
        public ChatMessage()
        {
            Viewed = new HashSet<Guid>();
        }

        public Guid Id { get; set; }

        public Guid EntityId { get; set; }

        public EntityType EntityType { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public DateTime Received { get; set; }

        public HashSet<Guid> Viewed { get; set; }
    }
}