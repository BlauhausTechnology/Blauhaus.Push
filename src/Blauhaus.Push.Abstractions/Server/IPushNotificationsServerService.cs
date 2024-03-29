﻿using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Responses;

namespace Blauhaus.Push.Abstractions.Server
{
    public interface IPushNotificationsServerService
    {
        Task<Response<IDeviceRegistration>> UpdateDeviceRegistrationAsync(IDeviceRegistration deviceRegistration, IPushNotificationsHub hub);
        Task<Response<IDeviceRegistration >> LoadRegistrationForUserDeviceAsync(string userId, string deviceId, IPushNotificationsHub hub);
        Task SendNotificationToUserAsync(IPushNotification notification, string userId, IPushNotificationsHub hub);
        Task SendNotificationToUserDeviceAsync(IPushNotification notification, string userId, string deviceIdentifier, IPushNotificationsHub hub);
        Task<Response> DeregisterUserDeviceAsync(string userId, string deviceIdentifier, IPushNotificationsHub hub);
    }
}