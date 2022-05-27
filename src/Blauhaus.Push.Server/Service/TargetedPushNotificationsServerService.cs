using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Extractors;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.Responses;
using Microsoft.Extensions.Logging;

namespace Blauhaus.Push.Server.Service
{
    //this does not work on the free tier
    public class TargetedPushNotificationsServerService : ITargetedPushNotificationsServerService
    {
        private readonly IAnalyticsLogger<TargetedPushNotificationsServerService> _logger;
        private readonly INativeNotificationExtractor _nativeNotificationExtractor;
        private readonly INotificationHubClientProxy _hubClientProxy;

        public TargetedPushNotificationsServerService(
            IAnalyticsLogger<TargetedPushNotificationsServerService> logger,
            INativeNotificationExtractor nativeNotificationExtractor,
            INotificationHubClientProxy hubClientProxy)
        {
            _logger = logger;
            _nativeNotificationExtractor = nativeNotificationExtractor;
            _hubClientProxy = hubClientProxy;
        }


        public async Task<Response> SendNotificationToTargetAsync(IPushNotification pushNotification, IDeviceTarget deviceTarget, IPushNotificationsHub hub)
        {
            try
            {
                _hubClientProxy.Initialize(hub);

                var nativeNotificationResult = _nativeNotificationExtractor.ExtractNotification(deviceTarget.Platform, pushNotification);
                if (nativeNotificationResult.IsFailure) return Response.Failure(nativeNotificationResult.Error);

                var notification = nativeNotificationResult.Value.Notification;
                var devices = new List<string>{ deviceTarget.PushNotificationServicesHandle };
                _logger.LogTrace("Native push notification extracted {@PushNotification}", notification);

                await _hubClientProxy.SendDirectNotificationAsync(notification, devices);
                return Response.Success();
            }
            catch (Exception e)
            {
                return _logger.LogErrorResponse(PushErrors.FailedToSendNotification, e);
            }
        }
    }
}