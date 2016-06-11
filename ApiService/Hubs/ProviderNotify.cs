using Common;
using GrainInterfaces;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiService.Hubs
{
    public class ProviderNotify : INotify
    {
        private readonly IHubContext _context;
        private readonly Guid _providerId;

        public ProviderNotify(Guid providerId)
        {
            _providerId = providerId;
            _context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        }

        public void NewMessages(IEnumerable<ChatNotification> notifications)
        {
            if (notifications.Count() > 0)
            {
                _context.Clients.User(_providerId.ToString()).notifyNewMessages(notifications);
            }
        }
    }
}