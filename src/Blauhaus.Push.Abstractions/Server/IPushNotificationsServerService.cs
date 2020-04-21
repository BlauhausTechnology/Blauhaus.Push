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
        
        /// <summary>
        /// This method sends directly to a PNS handle, ignoring all of the Registrations stuff. It is only supported at the Basic tier. 
        /// </summary>
        Task<Result> SendNotificationToDeviceAsync(IPushNotification notification, IDeviceTarget target, IPushNotificationsHub hub, CancellationToken token);

    }
}