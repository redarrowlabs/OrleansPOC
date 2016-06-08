using Common;
using GrainInterfaces;
using Grains.State;
using Orleans;
using Orleans.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider(ProviderName = "JsonStore")]
    public class ChatGrain : Grain<ChatState>, IChatGrain
    {
        public Task<IEnumerable<ChatMessage>> Messages()
        {
            return Task.FromResult(State.Messages.AsEnumerable());
        }

        public Task AddMessage(ChatMessage message)
        {
            State.Messages.Add(message);

            return base.WriteStateAsync();
        }
    }
}