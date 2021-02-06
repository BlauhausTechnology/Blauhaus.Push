using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Utils.Extensions;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Client.Common._Base;
using Newtonsoft.Json;

namespace Blauhaus.Push.Client.Common.Services
{
    public class UwpPushNotificationsClientService : BasePushNotificationsClientService
    {

        public UwpPushNotificationsClientService(
            ISecureStorageService secureStorageService,
            IAnalyticsService analyticsService,
            IPushNotificationTapHandler pushNotificationTapHandler) 
            : base(analyticsService, secureStorageService, pushNotificationTapHandler)
        {
        }

        public void HandleForegroundNotification(string uwpPayload)
        {

            using (var _ = AnalyticsService.StartTrace(this, "Foreground Push Notification"))
            {
                try
                {
                    PublishNotification(ExtractPushNotification(uwpPayload, true));
                }
                catch (Exception e)
                {
                    AnalyticsService.LogException(this, e);
                }
            }

        }

        public async Task HandleAppLaunchingAsync(string uwpPayload)
        {
            if (!string.IsNullOrWhiteSpace(uwpPayload))
            {
                using (var _ = AnalyticsService.StartTrace(this, "Push Notification Tapped"))
                {
                    try
                    {
                        await InvokeTapHandlersAsync(ExtractPushNotification(uwpPayload, false));
                    }
                    catch (Exception e)
                    {
                        AnalyticsService.LogException(this, e);
                        throw;
                    }
                }
            }
        }


        private IPushNotification ExtractPushNotification(string uwpPayload, bool isWrappedInXml)
        {
            AnalyticsService.TraceVerbose(this, "Extracting push notification", "Raw Notification", uwpPayload);

            if (isWrappedInXml)
            {
                uwpPayload = uwpPayload.ExtractValueBetweenText("<toast launch=\"", "\">");
            }

            var jsonString = uwpPayload.Replace("%22", "\'");
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

            var type = "";
            var title = "";
            var body = "";
            var dataProperties = new Dictionary<string, object>();

            foreach (var child in json)
            {
                if (child.Key.Equals("Title", StringComparison.InvariantCultureIgnoreCase))
                {
                    title = child.Value.ToString();
                }
                else if (child.Key.Equals("Body", StringComparison.InvariantCultureIgnoreCase))
                {
                    body = child.Value.ToString();
                }
                else if (child.Key.Equals("Template_Name", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = child.Value.ToString();
                }
                else if (int.TryParse(child.Value.ToString(), out var integerValue))
                {
                    dataProperties[child.Key] = integerValue;
                }
                else
                {
                    dataProperties[child.Key] = child.Value;
                }
            }
            
            var notification = new PushNotification(type, dataProperties, title, body);
            
            AnalyticsService.TraceVerbose(this, "Notification processed", notification.ToObjectDictionary());

            return notification;
        }
    }
}