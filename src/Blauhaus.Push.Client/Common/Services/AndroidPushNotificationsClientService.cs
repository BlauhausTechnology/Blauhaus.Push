using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Errors;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Client.Common.Base;
using Microsoft.Extensions.Logging;

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
            IAnalyticsLogger<AndroidPushNotificationsClientService> logger, 
            IPushNotificationTapHandler pushNotificationTapHandler) 
            : base(logger, secureStorageService, pushNotificationTapHandler)
        {
        }
        

        public void HandleForegroundNotification(Dictionary<string, object> androidPayload)
        {
            try
            {
                if(androidPayload ==null) throw new ArgumentNullException();
                PublishNotification(ExtractNotification(androidPayload));
            }
            catch (Exception e)
            {
                Logger.LogError(Error.Unexpected(), e);
            }
        }

        public async Task HandleNotificationTappedAsync(Dictionary<string, object>  androidPayload)
        {
            try
            {
                if (androidPayload == null)
                {
                    throw new ArgumentNullException();
                }

                if (androidPayload.ContainsKey("Template_Name"))
                {
                    await InvokeTapHandlersAsync(ExtractNotification(androidPayload));
                }
                else
                {
                    Logger.LogWarning("Payload is not a notification: ignoring {RawPushNotification}", androidPayload);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(Error.Unexpected(), e);
            }
        }

        private IPushNotification ExtractNotification(Dictionary<string, object> payload)
        {
            
   
            Logger.LogTrace("Extracting push notification {RawPushNotification}", payload);

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

                else if (string.Equals(notificationProperty.Key, "Template_Name", StringComparison.InvariantCultureIgnoreCase))
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
            Logger.LogTrace("Notification extracted: {@PushNotification}", pushNotification);

            return pushNotification;
        }
        
    }
}