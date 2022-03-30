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
using Blauhaus.Push.Client.Maui.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Blauhaus.Push.Client.Maui
{
    public class WindowsPushNotificationsClientService : BasePushNotificationsClientService
    {

        public WindowsPushNotificationsClientService(
            IAnalyticsLogger<WindowsPushNotificationsClientService> logger,
            ISecureStorageService secureStorageService,
            IPushNotificationTapHandler pushNotificationTapHandler) 
                : base(logger, secureStorageService, pushNotificationTapHandler)
        {
        }

        public void HandleForegroundNotification(string uwpPayload)
        {
            using var _ = Logger.LogTimed(LogLevel.Trace, "Handled foreground push notification");
            try
            {
                PublishNotification(ExtractPushNotification(uwpPayload, true));
            }
            catch (Exception e)
            {
                Logger.LogError(Error.Unexpected(e.Message), e);
            }
        }

        public async Task HandleAppLaunchingAsync(string uwpPayload)
        {
            if (!string.IsNullOrWhiteSpace(uwpPayload))
            {
                using var _ = Logger.LogTimed(LogLevel.Trace, "Handled background push notification tap");
                try
                {
                    await InvokeTapHandlersAsync(ExtractPushNotification(uwpPayload, false));
                }
                catch (Exception e)
                {
                    Logger.LogError(Error.Unexpected(e.Message), e);
                    throw;
                }
            }
        }


        private IPushNotification ExtractPushNotification(string uwpPayload, bool isWrappedInXml)
        {
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
            
            Logger.LogTrace("Notification processed: {@PushNotification}", notification);

            return notification;
        }
    }
}