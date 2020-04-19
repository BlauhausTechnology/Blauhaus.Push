using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Push.Abstractions;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server._Config;
using Blauhaus.Push.Server.Extensions;
using Blauhaus.Push.Server.HubClientProxy;
using CSharpFunctionalExtensions;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.Service
{
    public class PushNotificationsServerService : IPushNotificationsServerService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly INotificationHubClientProxy _hubClientProxy;

        public PushNotificationsServerService(
            IAnalyticsService analyticsService,
            INotificationHubClientProxy hubClientProxy)
        {
            _analyticsService = analyticsService;
            _hubClientProxy = hubClientProxy;
        }

        public async Task<Result<IDeviceRegistration>> UpdateDeviceRegistrationAsync(
            IDeviceRegistration deviceRegistration, 
            IPushNotificationsHub hub,
            CancellationToken token)
        {
            if (deviceRegistration.IsNotValid(this, _analyticsService, out var validationError))
            {
                return Result.Failure<IDeviceRegistration>(validationError);
            }

            using (var _ = _analyticsService.ContinueOperation(this, "Register device for push notifications", deviceRegistration.ToObjectDictionary()))
            {
                _hubClientProxy.Initialize(hub);

                var installation = new Installation
                {
                    PushChannel = deviceRegistration.PushNotificationServiceHandle,
                    InstallationId = deviceRegistration.DeviceIdentifier,
                    Tags = deviceRegistration.Tags,
                    Platform = deviceRegistration.Platform.ToNotificationPlatform(),
                    Templates = new Dictionary<string, InstallationTemplate>()
                };

                if (!string.IsNullOrEmpty(deviceRegistration.UserId))
                {
                    installation.Tags.Add($"UserId_{deviceRegistration.UserId}");
                }

                if (!string.IsNullOrEmpty(deviceRegistration.AccountId))
                {
                    installation.Tags.Add($"AccountId_{deviceRegistration.AccountId}");
                }

                foreach (var template in deviceRegistration.Templates)
                {
                    installation.Templates.Add(template.ToPlatform(deviceRegistration.Platform));
                }

                try
                {
                    await _hubClientProxy.CreateOrUpdateInstallationAsync(installation, token);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                return Result.Ok<IDeviceRegistration>(deviceRegistration);
            }
        }

        public async Task<Result<IDeviceRegistration>> LoadDeviceRegistrationAsync(
            string deviceIdentifier, 
            IPushNotificationsHub hub,
            CancellationToken token)
        {
            using (var _ = _analyticsService.ContinueOperation(this, "Load push notification registration for device", new Dictionary<string, object>{{"DeviceIdentifier", deviceIdentifier}}))
            {
                _hubClientProxy.Initialize(hub);

                var installationExists = await _hubClientProxy.InstallationExistsAsync(deviceIdentifier, token);

                if (!installationExists)
                {
                    var error = PushErrors.RegistrationDoesNotExist;
                    var code = error.Code;
                    return Result.Failure<IDeviceRegistration>(_analyticsService.TraceError(this, PushErrors.RegistrationDoesNotExist));
                }

                var installation = await _hubClientProxy.GetInstallationAsync(deviceIdentifier, token);

                var deviceRegistration = new DeviceRegistration
                {
                    Platform = installation.Platform.ToRuntimePlatform(),
                    DeviceIdentifier = installation.InstallationId,
                    UserId = installation.ExtractUserId(),
                    AccountId = installation.ExtractAccountId(),
                    PushNotificationServiceHandle = installation.PushChannel,
                    Tags = installation.ExtractTags(),
                    Templates = installation.ExtractTemplates()
                };

                //todo test
                _analyticsService.TraceVerbose(this, "Device registration loaded", deviceRegistration.ToObjectDictionary());

                return Result.Success<IDeviceRegistration>(deviceRegistration); 
            }
        }

        public async Task SendNotificationToUserAsync(
            IPushNotification notification, 
            string userId, 
            IPushNotificationsHub hub,
            CancellationToken token)
        {
            
            _hubClientProxy.Initialize(hub);

            using (var _ = _analyticsService.ContinueOperation(this, "Send push notification to user", new Dictionary<string, object>
                {{nameof(IPushNotification), notification}, {"UserId", userId }}))
            {

                var properties = notification.DataProperties.ToDictionary(notificationDataProperty => 
                    notificationDataProperty.Key, notificationDataProperty => notificationDataProperty.Value.ToString());

                var tags = new List<string>
                {
                    notification.Name,
                    $"UserId_{userId}"
                };

                if (!string.IsNullOrWhiteSpace(notification.Title))
                {
                    properties.Add("Title", notification.Title);
                }

                if (!string.IsNullOrWhiteSpace(notification.Body))
                {
                    properties.Add("Body", notification.Body);
                }

                var result = await _hubClientProxy.SendNotificationAsync(properties, tags, token);
            }
        }
    }
}