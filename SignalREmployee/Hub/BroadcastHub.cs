using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalREmployee
{
    public class BroadcastHub : Hub<IHubClient>
    {
        public override async Task OnConnectedAsync()
        {
            if(Context.User.Identity.Name == "Shan")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Transaction");
                await Groups.AddToGroupAsync(Context.ConnectionId, "Invoicing");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Invoicing");
            }
            
            await base.OnConnectedAsync();
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            //await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
            await Clients.Group(groupName).BroadcastMessage();
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            //await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
            await Clients.Group(groupName).BroadcastMessage();
        }
    }
}