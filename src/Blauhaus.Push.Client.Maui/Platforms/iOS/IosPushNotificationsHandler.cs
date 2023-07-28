using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Analytics.Abstractions.Service;
using UIKit;
using Foundation;
using UserNotifications;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Client.Maui.Ioc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Blauhaus.Push.Client.Maui
{
    public class IosPushNotificationHandler
    {
        private readonly IAnalyticsLogger<IosPushNotificationHandler> _logger;
        private readonly IPushNotificationsClientConfig _options;
        private readonly IosPushNotificationsClientService _pushNotificationsService;

        public IosPushNotificationHandler(
            IAnalyticsLogger<IosPushNotificationHandler> logger, 
            IPushNotificationsClientService pushNotificationsService,
            IPushNotificationsClientConfig options)
        {
            _pushNotificationsService = (IosPushNotificationsClientService) pushNotificationsService;
            _logger = logger;
            _options = options;
        }


        public async Task InitializeAsync(MauiUIApplicationDelegate appDelegate, NSDictionary options)
        {
            
            using var _ = _logger.BeginTimedScope(LogLevel.Debug, "iOs Push Notifications Initialization");

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (granted, error) =>
                    {
                        if (granted)
                        {
                            _logger.LogInformation("Permission granted for push notifications");
                            appDelegate.InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                        }
                        else
                        {
                            if(error?.Code != null)
                            {
                                _logger.LogWarning("Failed to get permission for push notifications: {Error}", error.Code.ToString());
                            }
                            else
                            {
                                _logger.LogWarning("Permission for push notifications not granted");
                            }
                        }
                    });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                const UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }

            if (options != null)
            {
                if (options.TryGetValue(new NSString("UIApplicationLaunchOptionsRemoteNotificationKey"), out var tapped))
                {
                    _logger.LogInformation("Notification payload available for new app session");
                    _logger.LogDebug("Payload: {UIApplicationLaunchOptionsRemoteNotificationKey}", tapped);
                    await HandleMessageReceivedAsync(UIApplicationState.Inactive, (NSDictionary) tapped);
                }
            }
        }

        public async void HandleNewToken(NSData deviceToken)
        {
            byte[] tokenBytes = deviceToken.ToArray();
            string tokenString = BitConverter.ToString(tokenBytes).Replace("-","");

            await _pushNotificationsService.UpdatePushNotificationServiceHandleAsync(tokenString);
        }

        public void HandleFailedRegistration(NSError deviceToken)
        {
            _logger.LogError(PushErrors.InvalidDeviceRegistration);
        }

        public async Task HandleMessageReceivedAsync(UIApplicationState applicationState, NSDictionary userInfo)
        {
            var data = NSJsonSerialization.Serialize(userInfo, 0, out var error);
            var json = data.ToString();
            
            if (applicationState == UIApplicationState.Active)
            {
                _pushNotificationsService.HandleForegroundNotification(json);
            }
            else
            {
                await _pushNotificationsService.HandleNotificationTappedAsync(json);
            }

        }
    }


}