using Android.Content;
using Android.App;
using Android.Gms.Common;
using Android.OS;
using Blauhaus.Analytics.Abstractions;
using Microsoft.Extensions.Options;
using Firebase.Messaging; 
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Maui.Ioc;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Blauhaus.Push.Client.Maui
{
    public class AndroidPushNotificationHandler
    {
        private readonly IAnalyticsLogger<AndroidPushNotificationHandler> _logger;

        private readonly AndroidPushNotificationsClientService _pushNotificationsService;
        private readonly IPushNotificationsClientConfig _options;

        public AndroidPushNotificationHandler(
            IAnalyticsLogger<AndroidPushNotificationHandler> logger, 
            IPushNotificationsClientService pushNotificationsService,
            IPushNotificationsClientConfig options)
        {
            _logger = logger;
            _pushNotificationsService = (AndroidPushNotificationsClientService)pushNotificationsService;
            _options = options;
}

        
        public void Initialize(Context context, Intent intent, NotificationManager notificationManager)
        {
            using var _ = _logger.LogTimed(LogLevel.Debug, "Initialized Android push notifications for Android SDK {AndroidSdkVersionCode}", Build.VERSION.SdkInt);

            var resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(context);

            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    var error = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                    _logger.LogWarning("Device does not have Google Play installed: {ErrorMessage}", error);
                }
                
                _logger.LogWarning("Device does not have Google Play installed: {ErrorCode}", resultCode);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(_options.NotificationHubName, _options.NotificationHubName, NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
                _logger.LogDebug("Created NotificationChannel for Android device using Android SDK {AndroidSdkVersionCode}", Build.VERSION.SdkInt);
            }
            else
            {
                _logger.LogDebug("Did not create NotificationChannel for Android device using Android SDK {AndroidSdkVersionCode}", Build.VERSION.SdkInt);
            }

            HandleNewIntent(intent);
        }

        public Task HandleNewTokenAsync(string updatedPnsHandle)
        {
            _logger.LogDebug("Push notification handle received from android: {PnsHandle}", updatedPnsHandle);
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