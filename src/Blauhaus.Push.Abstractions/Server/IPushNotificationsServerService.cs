using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.PushNotifications;
using CSharpFunctionalExtensions;

namespace Blauhaus.Push.Abstractions.Server
{
    public interface IPushNotificationsServerService
    {
        Task<Result<IDeviceRegistration >> UpdateDeviceRegistrationAsync(IDeviceRegistration deviceRegistration, CancellationToken token);
        Task<Result<IDeviceRegistration >> LoadDeviceRegistrationAsync(string deviceIdentifier, CancellationToken token);
        Task SendNotificationToUserAsync(IPushNotification notification, string userId, CancellationToken token);
    }
}