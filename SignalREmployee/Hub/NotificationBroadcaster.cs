using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalREmployee.Hub
{
    public class NotificationBroadcaster: INotificationBroadcaster
    {
        private IHubContext<BroadcastHub, IHubClient> broadcasterHubContext;

        public NotificationBroadcaster(IHubContext<BroadcastHub, IHubClient> broadcasterHubContext)
        {
            this.broadcasterHubContext = broadcasterHubContext;
        }

        public Task SendUpdate(string employeeName)
        {
            //return this.broadcasterHubContext.Clients.All.SendAsync("Update", employeeName);
            return this.broadcasterHubContext.Clients.All.BroadcastMessage();
        }
    }
}
