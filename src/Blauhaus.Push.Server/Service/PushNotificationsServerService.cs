using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Extensions;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.Responses;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Blauhaus.Push.Server.Service
{
    public class PushNotificationsServerService : IPushNotificationsServerService
    {
        private readonly IAnalyticsLogger<PushNotificationsServerService> _logger;
        private readonly INotificationHubClientProxy _hubClientProxy;

        public PushNotificationsServerService(
            IAnalyticsLogger<PushNotificationsServerService> logger,
            INotificationHubClientProxy hubClientProxy)
        {
            _logger = logger;
            _hubClientProxy = hubClientProxy;
        }

        public async Task<Response<IDeviceRegistration>> UpdateDeviceRegistrationAsync(IDeviceRegistration deviceRegistration, IPushNotificationsHub hub)
        {
            
            if (deviceRegistration.IsNotValid(this, _logger, out var validationError))
            {
                return Response.Failure<IDeviceRegistration>(validationError);
            }
            
            _logger.BeginTimedScope(LogLevel.Information, "Register device {DeviceIdentifier} for push notifications on {Platform}", deviceRegistration.DeviceIdentifier, deviceRegistration.Platform.Value);

            _hubClientProxy.Initialize(hub);

            var installation = new Installation
            {
                PushChannel = deviceRegistration.PushNotificationServiceHandle,
                InstallationId = deviceRegistration.UserId + "___" + deviceRegistration.DeviceIdentifier,
                Tags = deviceRegistration.Tags,
                Platform = deviceRegistration.Platform.ToNotificationPlatform(),
                Templates = new Dictionary<string, InstallationTemplate>()
            };

            installation.Tags.Add($"UserId_{deviceRegistration.UserId}");
            installation.Tags.Add($"DeviceIdentifier_{deviceRegistration.DeviceIdentifier}");

            if (!string.IsNullOrEmpty(deviceRegistration.AccountId))
            {
                installation.Tags.Add($"AccountId_{deviceRegistration.AccountId}");
            }

            foreach (var template in deviceRegistration.Templates)
            {
                installation.Templates.Add(template.ToPlatform(deviceRegistration.Platform));
            }
            
            await _hubClientProxy.CreateOrUpdateInstallationAsync(installation);
            
            _logger.LogInformation("Push notification registration updated: {PushNotificationInstallation}", installation);
            
            return Response.Success(deviceRegistration);
        }

        public async Task<Response<IDeviceRegistration>> LoadRegistrationForUserDeviceAsync(string userId, string deviceIdentifier, IPushNotificationsHub hub)
        {
            using var _ = _logger.BeginTimedScope(LogLevel.Debug, "Load push notification registration for user {UserId}  on device {DeviceIdentifier}", userId, deviceIdentifier);

            _hubClientProxy.Initialize(hub);

            var installationId = userId + "___" + deviceIdentifier;

            var installationExists = await _hubClientProxy.InstallationExistsAsync(installationId);

            if (!installationExists)
            {
                return _logger.LogErrorResponse<IDeviceRegistration>(PushErrors.RegistrationDoesNotExist);
            }

            var installation = await _hubClientProxy.GetInstallationAsync(installationId);

            var deviceRegistration = new DeviceRegistration
            {
                Platform = installation.Platform.ToRuntimePlatform(),
                DeviceIdentifier = installation.ExtractDeviceIdentifier(),
                UserId = installation.ExtractUserId(),
                AccountId = installation.ExtractAccountId(),
                PushNotificationServiceHandle = installation.PushChannel,
                Tags = installation.ExtractTags(),
                Templates = installation.ExtractTemplates()
            };

            _logger.LogTrace("Device Registration: {@DeviceRegistration}", deviceRegistration);

            return Response.Success<IDeviceRegistration>(deviceRegistration); 
        }
         
        public Task SendNotificationToUserAsync(IPushNotification notification, string userId, IPushNotificationsHub hub)
        {
            using var _ = _logger.BeginTimedScope(LogLevel.Information, "Send push notification {PushNoticiationName} to user {UserId}", notification.Name, userId);
            _logger.LogTrace("Push Notification: {@PushNotification}", notification);

            var tags = new List<string>
            {
                $"(UserId_{userId} && {notification.Name})"
            };

            return SendNotificationToTagsAsync(notification, hub, tags);
        }

        public Task SendNotificationToUserDeviceAsync(IPushNotification notification, string userId, string deviceIdentifier, IPushNotificationsHub hub)
        {
    
            using var _ = _logger.BeginTimedScope(LogLevel.Information, "Send push notification {PushNoticiationName} to user {UserId} on device {DeviceIdentifier}", notification.Name, userId, deviceIdentifier);
            _logger.LogTrace("Push Notification: {@PushNotification}", notification);
             
            var tags = new List<string>
            {
                $"(UserId_{userId} && {notification.Name} && DeviceIdentifier_{deviceIdentifier})"
            };

            return SendNotificationToTagsAsync(notification, hub, tags);

        }

        private async Task SendNotificationToTagsAsync(IPushNotification notification, IPushNotificationsHub hub, IEnumerable<string> tags)
        {
            _logger.LogDebug("Sending push notification {PushNoticiationName} to tags {@Tags}", notification.Name, tags);
            _logger.LogTrace("Push Notification: {@PushNotification}", notification);
            
            _hubClientProxy.Initialize(hub);

            var properties = notification.DataProperties.ToDictionary(notificationDataProperty => 
                notificationDataProperty.Key, notificationDataProperty => notificationDataProperty.Value.ToString());

            if (!string.IsNullOrWhiteSpace(notification.Title))
            {
                properties.Add("Title", notification.Title);
            }

            if (!string.IsNullOrWhiteSpace(notification.Body))
            {
                properties.Add("Body", notification.Body);
            }

            await _hubClientProxy.SendNotificationAsync(properties, tags);
        }

        public async Task<Response> DeregisterUserDeviceAsync(string userId, string deviceIdentifier, IPushNotificationsHub hub)
        {
            using var _ = _logger.BeginTimedScope(LogLevel.Information, "Deregister device {DeviceIdentifier} for user {UserId}", deviceIdentifier, userId);

            _hubClientProxy.Initialize(hub);

            var installationId = userId + "___" + deviceIdentifier;

            var installationExists = await _hubClientProxy.InstallationExistsAsync(installationId);
            if (!installationExists)
            {
                _logger.LogWarning("No installation exists for user device, so there is nothing to deregister");
                return Response.Success();
            }
        
            var installation = await _hubClientProxy.GetInstallationAsync(installationId);
            installation.Templates = new Dictionary<string, InstallationTemplate>();
            await _hubClientProxy.CreateOrUpdateInstallationAsync(installation);

            _logger.LogInformation("Templates cleared for push notifications installation: {@PushNotificationInstallation}", installation);

            return Response.Success();
        }
    }
}