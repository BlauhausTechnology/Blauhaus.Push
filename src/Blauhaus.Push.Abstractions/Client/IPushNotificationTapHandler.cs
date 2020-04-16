using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common;

namespace Blauhaus.Push.Abstractions.Client
{
    public interface IPushNotificationTapHandler
    {
        Task HandleTapAsync(IPushNotification tappedNotification);
    }
}