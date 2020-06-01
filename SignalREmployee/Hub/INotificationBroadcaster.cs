using System.Threading.Tasks;

namespace SignalREmployee.Hub
{
    public interface INotificationBroadcaster
    {
        Task SendUpdate(string employeeName);
    }
}
