using System.Threading.Tasks;

namespace Blauhaus.Push.Abstractions.Client
{
    public interface IPushNotificationTapHandler
    {
        Task HandleTapAsync(IPushNotification tappedNotification);
    }
}