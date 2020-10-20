using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Responses;
using CSharpFunctionalExtensions;

namespace Blauhaus.Push.Abstractions.Server
{
    public interface IPushNotificationsServerService
    {
        Task<Response<IDeviceRegistration>> UpdateDeviceRegistrationAsync(IDeviceRegistration deviceRegistration, IPushNotificationsHub hub, CancellationToken token);
        Task<Response<IDeviceRegistration >> LoadRegistrationForUserDeviceAsync(string userId, string deviceId, IPushNotificationsHub hub, CancellationToken token);
        Task SendNotificationToUserAsync(IPushNotification notification, string userId, IPushNotificationsHub hub, CancellationToken token);
        Task<Response> DeregisterUserDeviceAsync(string userId, string deviceIdentifier, IPushNotificationsHub hub, CancellationToken token);
    }
}