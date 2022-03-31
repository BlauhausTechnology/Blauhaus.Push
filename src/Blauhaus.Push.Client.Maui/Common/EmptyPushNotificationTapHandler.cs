using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Microsoft.Extensions.Logging;

namespace Blauhaus.Push.Client.Maui.Common
{
    public class EmptyPushNotificationTapHandler : IPushNotificationTapHandler
    {
        private readonly IAnalyticsLogger<EmptyPushNotificationTapHandler> _logger;

        public EmptyPushNotificationTapHandler(IAnalyticsLogger<EmptyPushNotificationTapHandler> logger)
        {
            _logger = logger;
        }


        public Task HandleTapAsync(IPushNotification tappedNotification)
        {
            _logger.LogTrace("No Push Notification Tap Handler configured");
            return Task.CompletedTask;
        }
    }
}