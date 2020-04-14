using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Push.Abstractions.Client;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions;

namespace Blauhaus.Push.Client.Common._Ioc
{
    public class EmptyPushNotificationTapHandler : IPushNotificationTapHandler
    {
        private readonly IAnalyticsService _analyticsService;

        public EmptyPushNotificationTapHandler(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }


        public Task HandleTapAsync(IPushNotification tappedNotification)
        {
            _analyticsService.Trace(this, "No Push Notification Tap Handler configured");
            return Task.CompletedTask;
        }
    }
}