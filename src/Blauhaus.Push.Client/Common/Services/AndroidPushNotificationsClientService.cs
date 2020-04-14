using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Push.Abstractions;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Common._Base;
using Newtonsoft.Json;

namespace Blauhaus.Push.Client.Common.Services
{
    public class AndroidPushNotificationsClientService : BasePushNotificationsClientService
    {
        private static readonly List<string> IgnoredFields =new List<string>
        {
            "google.delivered_priority",
            "google.sent_time",
            "google.ttl",
            "google.delivered_priority",
            "google.original_priority",
            "google.message_id",
            "collapse_key",
            "from",
        };

        public AndroidPushNotificationsClientService(
            ISecureStorageService secureStorageService,
            IAnalyticsService analyticsService, 
            IPushNotificationTapHandler pushNotificationTapHandler) 
            : base(analyticsService, secureStorageService, pushNotificationTapHandler)
        {
        }
        

        public void HandleForegroundNotification(Dictionary<string, object> androidPayload)
        {
            using (var _ = AnalyticsService.StartOperation(this, "Foreground Push Notification"))
            {
                try
                {
                    if(androidPayload ==null) throw new ArgumentNullException();
                    PublishNotification(ExtractNotification(androidPayload));
                }
                catch (Exception e)
                {
                    AnalyticsService.LogException(this, e);
                }
            }
        }

        public async Task HandleNotificationTappedAsync(Dictionary<string, object>  androidPayload)
        {
            using (var _ = AnalyticsService.StartOperation(this, "Push Notification Tapped"))
            {
                try
                {
                    if(androidPayload ==null) throw new ArgumentNullException();
                    await InvokeTapHandlersAsync(ExtractNotification(androidPayload));
                }
                catch (Exception e)
                {
                    AnalyticsService.LogException(this, e);
                }
            }
        }

        private IPushNotification ExtractNotification(Dictionary<string, object> payload)
        {
            
            AnalyticsService.TraceVerbose(this, "Extracting push notification", new Dictionary<string, object>
            {
                {"Raw Notification", payload }
            });

            var type = "";
            var title = "";
            var body = "";
            var data = new Dictionary<string, object>();

            foreach (var notificationProperty in payload)
            {
                if (IgnoredFields.Contains(notificationProperty.Key))
                {
                    continue;
                }

                if (string.Equals(notificationProperty.Key, "title", StringComparison.InvariantCultureIgnoreCase))
                {
                    title = notificationProperty.Value.ToString();
                }
                
                else if (string.Equals(notificationProperty.Key, "body", StringComparison.InvariantCultureIgnoreCase))
                {
                    body = notificationProperty.Value.ToString();
                }

                else if (string.Equals(notificationProperty.Key, "Template_Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = notificationProperty.Value.ToString();
                }

                else
                {
                    if (int.TryParse(notificationProperty.Value.ToString(), out var integerValue))
                    {
                        data[notificationProperty.Key] = integerValue;
                    }
                    else
                    {
                        data[notificationProperty.Key] = notificationProperty.Value;
                    }
                }
            }

            var pushNotification = new PushNotification(type, data, title, body); 
            AnalyticsService.TraceVerbose(this, "Notification processed", pushNotification.ToObjectDictionary());

            return pushNotification;
        }
        
    }
}