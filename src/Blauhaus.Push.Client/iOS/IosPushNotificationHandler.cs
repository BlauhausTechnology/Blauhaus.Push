﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Common._Ioc;
using Microsoft.Extensions.Options;
using Blauhaus.Analytics.Abstractions.Service;
using UIKit;
using Foundation;
using UserNotifications;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Push.Abstractions;
using Blauhaus.Push.Client.Common;
using Xamarin.Forms.Platform.iOS;
using Blauhaus.Push.Client.Common.Services;
using Newtonsoft.Json;
using Blauhaus.Push.Client.Common._Config;

namespace Blauhaus.Push.Client.iOS
{
    public class IosPushNotificationHandler
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IPushNotificationsClientConfig _config;
        private readonly IosPushNotificationsClientService _pushNotificationsService;

        public IosPushNotificationHandler(
            IAnalyticsService analyticsService, 
            IPushNotificationsClientService pushNotificationsService,
            IPushNotificationsClientConfig config)
        {
            _pushNotificationsService = (IosPushNotificationsClientService) pushNotificationsService;
            _analyticsService = analyticsService;
            _config = config;
        }


        public async Task InitializeAsync(FormsApplicationDelegate appDelegate, NSDictionary options)
        {

            using (var _ = _analyticsService.StartOperation(this, "iOs Push Notifications Initialization"))
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {
                    UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Sound,
                        (granted, error) =>
                        {
                            if (granted)
                            {
                                _analyticsService.TraceInformation(this, "Permission granted for push notifications on iOS10+");
                                appDelegate.InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                            }
                            else
                            {
                                _analyticsService.TraceWarning(this, "Failed to get permission for push notifications on iOS10+", "ErrorCode", error.Code.ToString());
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
                        _analyticsService.TraceVerbose(this, "Notification payload available for new app session");
                        await HandleMessageReceivedAsync(UIApplicationState.Inactive, (NSDictionary) tapped);
                    }
                }
            }
            
        }

        public async void HandleNewToken(NSData deviceToken)
        {
            var tokenBytes = deviceToken.ToArray();
            var tokenString = BitConverter.ToString(tokenBytes).Replace("-","");

            await _pushNotificationsService.UpdatePushNotificationServiceHandleAsync(tokenString);
        }

        public void HandleFailedRegistration(NSError deviceToken)
        {
            _analyticsService.TraceError(this, PushErrors.InvalidDeviceRegistration);
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