using System.Threading.Tasks;

namespace SignalREmployee
{
    public interface IHubClient
    {
        Task BroadcastMessage();
    }
}