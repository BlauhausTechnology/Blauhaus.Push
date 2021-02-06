using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Client.Common._Base;
using Newtonsoft.Json;

namespace Blauhaus.Push.Client.Common.Services
{
    public class IosPushNotificationsClientService : BasePushNotificationsClientService
    {
        
        public IosPushNotificationsClientService(
            ISecureStorageService secureStorageService,
            IAnalyticsService analyticsService,
            IPushNotificationTapHandler pushNotificationTapHandler) 
            : base(analyticsService, secureStorageService, pushNotificationTapHandler)
        {
        }

        public void HandleForegroundNotification(string iosPayload)
        {
            using (var _ = AnalyticsService.StartTrace(this, "Foreground Push Notification"))
            {
                try
                {
                    PublishNotification(ExtractNotification(iosPayload));
                }
                catch (Exception e)
                {
                    AnalyticsService.LogException(this, e);
                }
            }
        }

        public async Task HandleNotificationTappedAsync(string iosPayload)
        {
            using (var _ = AnalyticsService.StartTrace(this, "Push Notification Tapped"))
            {
                try
                {
                    await InvokeTapHandlersAsync(ExtractNotification(iosPayload));
                }
                catch (Exception e)
                {
                    AnalyticsService.LogException(this, e);
                }
            }
        }

        private IPushNotification ExtractNotification(string iosPayload)
        {
            AnalyticsService.TraceVerbose(this, "Extracting push notification", "Raw Notification", iosPayload);

            var type = "";
            var title = "";
            var body = "";
            var data = new Dictionary<string, object>();

            var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(iosPayload);

            foreach (var notificationProperty in payload)
            {
                if (notificationProperty.Key == "aps")
                {
                    var visibleNotificationProperties = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(notificationProperty.Value.ToString());
                    if (visibleNotificationProperties.TryGetValue("alert", out var visibleAlert))
                    {
                        if (visibleAlert.TryGetValue("title", out var visibleTitle))
                        {
                            title = visibleTitle.ToString();
                        }

                        if (visibleAlert.TryGetValue("body", out var visibleBody))
                        {
                            body = visibleBody.ToString();
                        }
                    }
                }
                else
                {
                    if (notificationProperty.Key.Equals("Template_Name", StringComparison.InvariantCultureIgnoreCase))
                    {
                        type = notificationProperty.Value.ToString();
                    }
                    else if (int.TryParse(notificationProperty.Value.ToString(), out var integerValue))
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