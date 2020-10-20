using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Extractors;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.Responses;
using CSharpFunctionalExtensions;

namespace Blauhaus.Push.Server.Service
{
    //this does not work on the free tier
    public class TargetedPushNotificationsServerService : ITargetedPushNotificationsServerService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly INativeNotificationExtractor _nativeNotificationExtractor;
        private readonly INotificationHubClientProxy _hubClientProxy;

        public TargetedPushNotificationsServerService(
            IAnalyticsService analyticsService,
            INativeNotificationExtractor nativeNotificationExtractor,
            INotificationHubClientProxy hubClientProxy)
        {
            _analyticsService = analyticsService;
            _nativeNotificationExtractor = nativeNotificationExtractor;
            _hubClientProxy = hubClientProxy;
        }


        public async Task<Response> SendNotificationToTargetAsync(IPushNotification pushNotification, IDeviceTarget deviceTarget, IPushNotificationsHub hub, CancellationToken token)
        {
            _analyticsService.TraceVerbose(this, "Send push notification to device", new Dictionary<string, object>
                {{nameof(PushNotification), pushNotification}, {nameof(DeviceTarget), deviceTarget}});

            try
            {
                _hubClientProxy.Initialize(hub);

                var nativeNotificationResult = _nativeNotificationExtractor.ExtractNotification(deviceTarget.Platform, pushNotification);
                if (nativeNotificationResult.IsFailure) return Response.Failure(nativeNotificationResult.Error);

                var notification = nativeNotificationResult.Value.Notification;
                var devices = new List<string>{ deviceTarget.PushNotificationServicesHandle };
                _analyticsService.Trace(this, "Native push notification extracted", LogSeverity.Verbose, notification.ToObjectDictionary());

                await _hubClientProxy.SendDirectNotificationAsync(notification, devices, token);
                return Response.Success();
            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse(this, e, PushErrors.FailedToSendNotification);
            }
        }
    }
}