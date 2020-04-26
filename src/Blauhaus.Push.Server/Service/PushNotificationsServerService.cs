using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Results;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server._Config;
using Blauhaus.Push.Server.Extensions;
using Blauhaus.Push.Server.Extractors;
using Blauhaus.Push.Server.HubClientProxy;
using CSharpFunctionalExtensions;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.Service
{
    public class PushNotificationsServerService : IPushNotificationsServerService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly INativeNotificationExtractor _nativeNotificationExtractor;
        private readonly INotificationHubClientProxy _hubClientProxy;

        public PushNotificationsServerService(
            IAnalyticsService analyticsService,
            INativeNotificationExtractor nativeNotificationExtractor,
            INotificationHubClientProxy hubClientProxy)
        {
            _analyticsService = analyticsService;
            _nativeNotificationExtractor = nativeNotificationExtractor;
            _hubClientProxy = hubClientProxy;
        }

        public async Task<Result<IDeviceRegistration>> UpdateDeviceRegistrationAsync(
            IDeviceRegistration deviceRegistration, IPushNotificationsHub hub, CancellationToken token)
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
                    InstallationId = deviceRegistration.UserId + "___" + deviceRegistration.DeviceIdentifier,
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
                
                await _hubClientProxy.CreateOrUpdateInstallationAsync(installation, token);
                
                _analyticsService.TraceVerbose(this, "Push notification registration updated", new Dictionary<string, object>
                {
                    {"InstallationId", installation.InstallationId },
                    {"Platform", installation.Platform },
                    {"PushChannel", installation.PushChannel },
                    {"Tags", installation.Tags },
                });
                
                return Result.Ok<IDeviceRegistration>(deviceRegistration);
            }
        }

        public async Task<Result<IDeviceRegistration>> LoadRegistrationForUserDeviceAsync(
            string userId, string deviceIdentifier, IPushNotificationsHub hub, CancellationToken token)
        {
            using (var _ = _analyticsService.ContinueOperation(this, "Load push notification registration for user device", 
                new Dictionary<string, object>
                {
                    {"DeviceIdentifier", deviceIdentifier},
                    {"UserId", userId }
                }))
            {
                _hubClientProxy.Initialize(hub);

                var installationId = userId + "___" + deviceIdentifier;

                var installationExists = await _hubClientProxy.InstallationExistsAsync(installationId, token);

                if (!installationExists)
                {
                    var error = PushErrors.RegistrationDoesNotExist;
                    var code = error.Code;
                    return Result.Failure<IDeviceRegistration>(_analyticsService.TraceError(this, PushErrors.RegistrationDoesNotExist));
                }

                var installation = await _hubClientProxy.GetInstallationAsync(installationId, token);

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

                //todo test
                _analyticsService.TraceVerbose(this, "Device registration loaded", deviceRegistration.ToObjectDictionary());

                return Result.Success<IDeviceRegistration>(deviceRegistration); 
            }
        }
         

        public async Task SendNotificationToUserAsync(
            IPushNotification notification, string userId, IPushNotificationsHub hub, CancellationToken token)
        {
            
            _hubClientProxy.Initialize(hub);

            using (var _ = _analyticsService.ContinueOperation(this, "Send push notification to user", new Dictionary<string, object>
                {{nameof(IPushNotification), notification}, {"UserId", userId }}))
            {

                var properties = notification.DataProperties.ToDictionary(notificationDataProperty => 
                    notificationDataProperty.Key, notificationDataProperty => notificationDataProperty.Value.ToString());

                var tags = new List<string>
                {
                    $"(UserId_{userId} && {notification.Name})"
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
         
        public async Task<Result> DeregisterUserDeviceAsync(string userId, string deviceIdentifier, IPushNotificationsHub hub, CancellationToken token)
        {
            _hubClientProxy.Initialize(hub);

            using (var _ = _analyticsService.ContinueOperation(this, "Deregister user device",
                new Dictionary<string, object>
                {
                    {"DeviceIdentifier", deviceIdentifier},
                    {"UserId", userId}
                }))
            {
                var installationId = userId + "___" + deviceIdentifier;

                var installationExists = await _hubClientProxy.InstallationExistsAsync(installationId, token);
                if (!installationExists)
                {
                    return Result.Failure<IDeviceRegistration>(_analyticsService.TraceError(this, PushErrors.RegistrationDoesNotExist));
                }
            
                var installation = await _hubClientProxy.GetInstallationAsync(installationId, token);
                installation.Templates = new Dictionary<string, InstallationTemplate>();
                await _hubClientProxy.CreateOrUpdateInstallationAsync(installation, token);

                _analyticsService.TraceVerbose(this, "Templates cleared for push notifications registration", new Dictionary<string, object>
                {
                    {"InstallationId", installation.InstallationId },
                    {"Platform", installation.Platform },
                    {"PushChannel", installation.PushChannel },
                    {"Tags", installation.Tags },
                });

                return Result.Success();
            }


        }
    }
}