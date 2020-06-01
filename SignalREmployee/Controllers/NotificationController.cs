using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalREmployee.Hub;

namespace SignalREmployee.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        //private readonly INotificationBroadcaster _notificationBroadcaster;
        public NotificationController(IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            _hubContext = hubContext;
            //_notificationBroadcaster = notificationBroadcaster;
        }

        [HttpGet]
        //[Authorize]
        public async Task<bool> Get(string employeeName)
        {
            //this._notificationBroadcaster.SendUpdate(employeeName);
            _hubContext.Clients.All.BroadcastMessage();
            return true;
        }
    }
}