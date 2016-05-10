using System;

namespace Client.Models
{
    public class UserChatMessage
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public DateTime Received { get; set; }
    }
}