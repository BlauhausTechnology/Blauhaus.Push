using System;
using Microsoft.Extensions.Options;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.Abstractions.Extensions;
using Windows.Networking.PushNotifications;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Core;
using System.Diagnostics;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Errors;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Maui.Ioc;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Blauhaus.Push.Client.Maui
{
    public class WindowsPushNotificationHandler
    {
        
        private readonly IAnalyticsLogger<WindowsPushNotificationHandler> _logger;
        private readonly IPushNotificationsClientConfig _options;
        private readonly WindowsPushNotificationsClientService _pushNotificationsService;
        private bool _appIsActive;
        private static bool _appIsInitialized;
        private PushNotificationChannel _channel;

        public WindowsPushNotificationHandler(
            IAnalyticsLogger<WindowsPushNotificationHandler> logger, 
            IPushNotificationsClientService pushNotificationsService,
            IPushNotificationsClientConfig options)
        {
            _pushNotificationsService = (WindowsPushNotificationsClientService)pushNotificationsService;
            _logger = logger;
            _options = options;
        }

        public async Task InitializeAsync(Window currentWindow)
        {
            if (!_appIsInitialized)
            {
                using var _ = _logger.BeginTimedScope(LogLevel.Debug, "UWP Push Notifications Initialization");
                try
                {
                    //currentWindow.Activated += HandleWindowActivated;

                    _channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                    _channel.PushNotificationReceived += HandlePushNotificationReceived;
                
                    await _pushNotificationsService.UpdatePushNotificationServiceHandleAsync(_channel.Uri);
                
                    _appIsInitialized = true;
                }
                catch (Exception e)
                {
                    _logger.LogError(Error.Unexpected(), e);
                    throw;
                }
            }
        }
        
        private void HandlePushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            if (args.NotificationType == PushNotificationType.Toast)
            {
                try
                {
                    _pushNotificationsService.HandleForegroundNotification(args.ToastNotification.Content.GetXml());

                    if (_appIsActive)
                    {
                        //app is in focus so we don't need to show toast?
                        args.Cancel = true;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(Error.Unexpected(), e);
                    throw;
                }
            }
            else
            {
                _logger.LogWarning("Received a non-toast push notification of type {NotificationType}, ignoring...", args.NotificationType.ToString());
            }
        }

        public Task HandleAppLaunching(LaunchActivatedEventArgs launchActivatedEventArgs)
        {
            return _pushNotificationsService.HandleAppLaunchingAsync(launchActivatedEventArgs.Arguments);
        }

        
        private void HandleWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                Debug.WriteLine("DEACTIVATED");
                _appIsActive = false;
            }
            else
            {
                Debug.WriteLine("Activated");
                _appIsActive = true;
            }
        }

    }
}