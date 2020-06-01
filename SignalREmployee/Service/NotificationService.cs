using SignalREmployee.Hub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalREmployee.Service
{
    public class NotificationService
    {
        private INotificationBroadcaster boradcaster;

        public NotificationService(INotificationBroadcaster broadcaster)
        {
            this.boradcaster = broadcaster;
        }

        public void SendNotification(string employeeName)
        {
            this.boradcaster.SendUpdate(employeeName);
        }
    }
}
