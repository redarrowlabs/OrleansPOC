using Common;
using System.Collections.Generic;

namespace Grains.State
{
    public class ChatState
    {
        public ChatState()
        {
            Messages = new List<ChatMessage>();
        }

        public List<ChatMessage> Messages { get; set; }
    }
}