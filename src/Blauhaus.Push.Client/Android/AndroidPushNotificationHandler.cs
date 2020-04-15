using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Content;
using Blauhaus.Analytics.Abstractions.Service;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using System.Linq;
using Android.OS;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Push.Client.Common._Ioc;
using Microsoft.Extensions.Options;
using Firebase.Messaging;
using Blauhaus.Push.Client.Common.Services;
using Blauhaus.Push.Abstractions.Client;
using Newtonsoft.Json;
using Blauhaus.Push.Client.Common._Config;

namespace Blauhaus.Push.Client.Android
{
    public class AndroidPushNotificationHandler
    {

        private readonly IAnalyticsService _analyticsService;
        private readonly IPushNotificationsClientConfig _config;
        private readonly AndroidPushNotificationsClientService _pushNotificationsService;

        public AndroidPushNotificationHandler(
            IAnalyticsService analyticsService, 
            IPushNotificationsClientService pushNotificationsService,
            IPushNotificationsClientConfig config)
        {
            _pushNotificationsService = (AndroidPushNotificationsClientService)pushNotificationsService;
            _analyticsService = analyticsService;
            _config = config;
        }

        
        public void Initialize(Context context, Intent intent, NotificationManager notificationManager)
        {
            using (var _ = _analyticsService.StartOperation(this, "Android Push Notifications Initialization"))
            {
                var resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(context);

                if (resultCode != ConnectionResult.Success)
                {
                    if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    {
                        var error = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                        _analyticsService.TraceWarning(this, "Device does not have Google Play installed", "Error", error);
                    }

                    _analyticsService.TraceWarning(this, "Device does not have Google Play installed", "ResultCode", resultCode.ToString());
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channel = new NotificationChannel(_config.NotificationHubName, _config.NotificationHubName, NotificationImportance.Default);
                    notificationManager.CreateNotificationChannel(channel);
                    _analyticsService.TraceVerbose(this, "Created NotificationChannel for Android device", "BuildVersion", Build.VERSION.SdkInt.ToString());
                }

                HandleNewIntent(intent);
            }
        }

        public Task HandleNewTokenAsync(string updatedPnsHandle)
        {
            return _pushNotificationsService.UpdatePushNotificationServiceHandleAsync(updatedPnsHandle);
        }

        public void HandleMessageReceived(RemoteMessage message)
        {
            var data = new Dictionary<string, object>();
            foreach (var value in message.Data)
            {
                data[value.Key] = value.Value;
            }
            
            _pushNotificationsService.HandleForegroundNotification(data);

        }

        public async void HandleNewIntent(Intent intent)
        {
            if (intent.Extras != null)
            {
                var data = intent.Extras.KeySet()
                    .ToDictionary<string, string, object>(key => key, key => intent.Extras.Get(key));

                await _pushNotificationsService.HandleNotificationTappedAsync(data);
            }

        }

    }
}