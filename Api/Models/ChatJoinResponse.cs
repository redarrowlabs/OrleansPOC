using Common;
using System.Collections.Generic;

namespace Api.Models
{
    public class ChatJoinResponse
    {
        public IEnumerable<ChatMessage> Messages { get; set; }

        public IEnumerable<ChatEntity> Users { get; set; }
    }
}