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
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Common.Config;
using Blauhaus.Push.Client.Services;

namespace Blauhaus.Push.Client.UWP
{
    public class UwpPushNotificationHandler
    {
        
        private readonly IAnalyticsService _analyticsService;
        private readonly IPushNotificationsClientConfig _config;
        private readonly UwpPushNotificationsClientService _pushNotificationsService;
        private bool _appIsActive;
        private static bool _appIsInitialized;
        private PushNotificationChannel _channel;

        public UwpPushNotificationHandler(
            IAnalyticsService analyticsService, 
            IPushNotificationsClientService pushNotificationsService,
            IPushNotificationsClientConfig config)
        {
            _pushNotificationsService = (UwpPushNotificationsClientService)pushNotificationsService;
            _analyticsService = analyticsService;
            _config = config;
        }

        public async Task InitializeAsync(Window currentWindow)
        {
            if (!_appIsInitialized)
            {
                using (var _ = _analyticsService.StartTrace(this, "UWP Push Notifications Initialization"))
                {
                    try
                    {
                        currentWindow.Activated += HandleWindowActivated;

                        _channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                        _channel.PushNotificationReceived += HandlePushNotificationReceived;
                
                        await _pushNotificationsService.UpdatePushNotificationServiceHandleAsync(_channel.Uri);
                
                        _appIsInitialized = true;
                    }
                    catch (Exception e)
                    {
                        _analyticsService.LogException(this, e);
                        throw;
                    }
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
                    _analyticsService.LogException(this, e);
                    throw;
                }
            }
            else
            {
                _analyticsService.TraceWarning(this, "Received a non-toast push notification, ignoring...", "Name", args.NotificationType.ToString());
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