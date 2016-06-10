using Common;
using Orleans;
using System.Collections.Generic;

namespace GrainInterfaces
{
    public interface INotify : IGrainObserver
    {
        void NewMessages(IEnumerable<ChatNotification> notifications);
    }
}