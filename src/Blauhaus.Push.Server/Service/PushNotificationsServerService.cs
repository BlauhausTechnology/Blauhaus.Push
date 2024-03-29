﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Extensions;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.Responses;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;

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

        public async Task<Response<IDeviceRegistration>> UpdateDeviceRegistrationAsync(IDeviceRegistration deviceRegistration, IPushNotificationsHub hub)
        {
            
            if (deviceRegistration.IsNotValid(this, _analyticsService, out var validationError))
            {
                return Response.Failure<IDeviceRegistration>(validationError);
            }
            
            _analyticsService.TraceVerbose(this, "Register device for push notifications", deviceRegistration.ToObjectDictionary());

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
            
            _analyticsService.TraceVerbose(this, "Push notification registration updated", new Dictionary<string, object>
            {
                {"InstallationId", installation.InstallationId },
                {"Platform", installation.Platform },
                {"PushChannel", installation.PushChannel },
                {"Tags", installation.Tags },
            });
            
            return Response.Success(deviceRegistration);
        }

        public async Task<Response<IDeviceRegistration>> LoadRegistrationForUserDeviceAsync(string userId, string deviceIdentifier, IPushNotificationsHub hub)
        {
            _analyticsService.TraceVerbose(this, "Load push notification registration for user device",
                new Dictionary<string, object>
                {
                    {"DeviceIdentifier", deviceIdentifier},
                    {"UserId", userId}
                });

            _hubClientProxy.Initialize(hub);

            var installationId = userId + "___" + deviceIdentifier;

            var installationExists = await _hubClientProxy.InstallationExistsAsync(installationId);

            if (!installationExists)
            {
                return _analyticsService.TraceErrorResponse<IDeviceRegistration>(this, PushErrors.RegistrationDoesNotExist);
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

            _analyticsService.TraceVerbose(this, "Device registration loaded", deviceRegistration.ToObjectDictionary());

            return Response.Success<IDeviceRegistration>(deviceRegistration); 
            
        }
         
        public Task SendNotificationToUserAsync(IPushNotification notification, string userId, IPushNotificationsHub hub)
        {
            _analyticsService.TraceVerbose(this, "Sending push notification to user", new Dictionary<string, object> {{nameof(IPushNotification), notification}, {"UserId", userId}});

            var tags = new List<string>
            {
                $"(UserId_{userId} && {notification.Name})"
            };

            return SendNotificationToTagsAsync(notification, hub, tags);


        }

        public Task SendNotificationToUserDeviceAsync(IPushNotification notification, string userId, string deviceIdentifier, IPushNotificationsHub hub)
        {
            _analyticsService.TraceVerbose(this, "Sending push notification to user device", new Dictionary<string, object>
            {
                {nameof(IPushNotification), notification}, 
                {"UserId", userId},
                {"DeviceIdentifier", deviceIdentifier},
            });
             
            var tags = new List<string>
            {
                $"(UserId_{userId} && {notification.Name} && DeviceIdentifier_{deviceIdentifier})"
            };

            return SendNotificationToTagsAsync(notification, hub, tags);

        }

        private async Task SendNotificationToTagsAsync(IPushNotification notification, IPushNotificationsHub hub, IEnumerable<string> tags)
        {
            
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
            _analyticsService.TraceVerbose(this, "Deregister user device",
                new Dictionary<string, object>
                {
                    {"DeviceIdentifier", deviceIdentifier},
                    {"UserId", userId}
                });

            _hubClientProxy.Initialize(hub);

            var installationId = userId + "___" + deviceIdentifier;

            var installationExists = await _hubClientProxy.InstallationExistsAsync(installationId);
            if (!installationExists)
            {
                _analyticsService.TraceWarning(this, "No installation exists for user device, so there is nothing to deregister");
                return Response.Success();
            }
        
            var installation = await _hubClientProxy.GetInstallationAsync(installationId);
            installation.Templates = new Dictionary<string, InstallationTemplate>();
            await _hubClientProxy.CreateOrUpdateInstallationAsync(installation);

            _analyticsService.TraceVerbose(this, "Templates cleared for push notifications registration", new Dictionary<string, object>
            {
                {"InstallationId", installation.InstallationId },
                {"Platform", installation.Platform },
                {"PushChannel", installation.PushChannel },
                {"Tags", installation.Tags },
            });

            return Response.Success();
        }
    }
}