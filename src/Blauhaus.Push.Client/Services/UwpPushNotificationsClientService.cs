using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Utils.Extensions;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Errors;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Client.Common.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Blauhaus.Push.Client.Services
{
    public class UwpPushNotificationsClientService : BasePushNotificationsClientService
    {

        public UwpPushNotificationsClientService(
            ISecureStorageService secureStorageService,
            IAnalyticsLogger<UwpPushNotificationsClientService> logger,
            IPushNotificationTapHandler pushNotificationTapHandler)
            : base(logger, secureStorageService, pushNotificationTapHandler)
        {
        }

        public void HandleForegroundNotification(string uwpPayload)
        {

            try
            {
                PublishNotification(ExtractPushNotification(uwpPayload, true));
            }
            catch (Exception e)
            {
                Logger.LogError(Error.Unexpected(), e);
            }

        }

        public async Task HandleAppLaunchingAsync(string uwpPayload)
        {
            if (!string.IsNullOrWhiteSpace(uwpPayload))
                try
                {
                    await InvokeTapHandlersAsync(ExtractPushNotification(uwpPayload, false));
                }
                catch (Exception e)
                {
                    Logger.LogError(Error.Unexpected(), e);
                    throw;
                }
        }


        private IPushNotification ExtractPushNotification(string uwpPayload, bool isWrappedInXml)
        {
            Logger.LogTrace("Extracting push notification {RawPushNotification}", uwpPayload);

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

            Logger.LogTrace("Notification extracted: {@PushNotification}", notification);

            return notification;
        }
    }
}