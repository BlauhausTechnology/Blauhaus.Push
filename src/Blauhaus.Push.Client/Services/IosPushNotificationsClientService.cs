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
using Newtonsoft.Json;

namespace Blauhaus.Push.Client.Services
{
    public class IosPushNotificationsClientService : BasePushNotificationsClientService
    {

        public IosPushNotificationsClientService(
            ISecureStorageService secureStorageService,
            IAnalyticsLogger<IosPushNotificationsClientService> logger,
            IPushNotificationTapHandler pushNotificationTapHandler)
            : base(logger, secureStorageService, pushNotificationTapHandler)
        {
        }

        public void HandleForegroundNotification(string iosPayload)
        {
            try
            {
                PublishNotification(ExtractNotification(iosPayload));
            }
            catch (Exception e)
            {
                Logger.LogError(Error.Unexpected(), e);
            }
        }

        public async Task HandleNotificationTappedAsync(string iosPayload)
        {
            try
            {
                await InvokeTapHandlersAsync(ExtractNotification(iosPayload));
            }
            catch (Exception e)
            {
                Logger.LogError(Error.Unexpected(), e);
            }
        }

        private IPushNotification ExtractNotification(string iosPayload)
        {
            Logger.LogTrace("Extracting push notification {RawPushNotification}", iosPayload);

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
            Logger.LogTrace("Notification extracted: {@PushNotification}", pushNotification);

            return pushNotification;
        }

    }
}