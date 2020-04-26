using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Notifications;
using CSharpFunctionalExtensions;

namespace Blauhaus.Push.Abstractions.Server
{
    public interface IPushNotificationsServerService
    {
        Task<Result<IDeviceRegistration>> UpdateDeviceRegistrationAsync(IDeviceRegistration deviceRegistration, IPushNotificationsHub hub, CancellationToken token);
        Task<Result<IDeviceRegistration >> LoadRegistrationForUserDeviceAsync(string userId, string deviceId, IPushNotificationsHub hub, CancellationToken token);
        Task SendNotificationToUserAsync(IPushNotification notification, string userId, IPushNotificationsHub hub, CancellationToken token);
        Task<Result> DeregisterUserDeviceAsync(string userId, string deviceIdentifier, IPushNotificationsHub hub, CancellationToken token);
    }
}