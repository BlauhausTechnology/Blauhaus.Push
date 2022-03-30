using Blauhaus.Analytics.Abstractions;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Microsoft.Extensions.Logging;

namespace Blauhaus.Push.Client.Maui.Common
{
    public abstract class BasePushNotificationsClientService : BasePublisher, IPushNotificationsClientService
    {
        private const string PnsHandleKey = "PnsHandle";
        private string _currentPnsHandle;

        protected readonly IAnalyticsLogger Logger;
        private readonly ISecureStorageService _secureStorageService;
        private readonly IPushNotificationTapHandler _pushNotificationTapHandler;

        protected BasePushNotificationsClientService(
            IAnalyticsLogger logger,
            ISecureStorageService secureStorageService,
            IPushNotificationTapHandler pushNotificationTapHandler)
        {
            Logger = logger;
            _secureStorageService = secureStorageService;
            _pushNotificationTapHandler = pushNotificationTapHandler;

            NewNotificationEvent += HandleNewForegroundNotification;
        }

       
        public async ValueTask<string> GetPushNotificationServiceHandleAsync()
        {
            if (string.IsNullOrWhiteSpace(_currentPnsHandle))
            {
                var storedHandle = await _secureStorageService.GetAsync(PnsHandleKey);
                if (string.IsNullOrWhiteSpace(storedHandle))
                {
                    _currentPnsHandle = string.Empty;
                    Logger.LogInformation("No PnsHandle found in secure storage");
                }
                else
                {
                    _currentPnsHandle = storedHandle;
                    Logger.LogInformation("PnsHandle found in secure storage: {PnsHandle}", _currentPnsHandle);
                }
            }
            return _currentPnsHandle;
        }

        public async Task UpdatePushNotificationServiceHandleAsync(string pnsHandle)
        {
            if (string.IsNullOrWhiteSpace(pnsHandle))
            {
                throw new ArgumentException("Push notification service handle cannot be empty");
            }

            var currentHandle = await GetPushNotificationServiceHandleAsync();
            if (pnsHandle != currentHandle)
            {
                if (string.IsNullOrWhiteSpace(currentHandle))
                {
                    Logger.LogInformation("New PnsHandle saved: {PnsHandle}", pnsHandle);
                }
                else
                {
                    Logger.LogInformation("PnsHandle updated from: {OldPnsHandle} to {PnsHandle}", currentHandle, pnsHandle);
                }
                
                _currentPnsHandle = pnsHandle;
                await _secureStorageService.SetAsync(PnsHandleKey, pnsHandle);
            }
        }
        
        
        #region IAsyncPublisher
        
        public Task<IDisposable> SubscribeAsync(Func<IPushNotification, Task> handler, Func<IPushNotification, bool>? filter = null)
        {
            return Task.FromResult(base.AddSubscriber(handler, filter));
        }
        
        private async void HandleNewForegroundNotification(object sender, NewNotificationEventArgs e)
        {
            Logger.LogInformation("Foreground {PushNotificationName} notification being published. Title: {PushNotificationTitle}", e.NewNotification.Name, e.NewNotification.Title);
            await UpdateSubscribersAsync(e.NewNotification);
        }

        #endregion
        
         

        #region New Notification event

        public event EventHandler<NewNotificationEventArgs> NewNotificationEvent;
        
        protected void PublishNotification(IPushNotification notification)
        {
            NewNotificationEvent?.Invoke(this, new NewNotificationEventArgs(notification));
        }

        #endregion
        
        #region Tapped Notification
        
        protected async Task InvokeTapHandlersAsync(IPushNotification pushNotification)
        {
            Logger.LogInformation("Background {PushNotificationName} notification tapped. Title: {PushNotificationTitle}", pushNotification.Name, pushNotification.Title);
            await _pushNotificationTapHandler.HandleTapAsync(pushNotification);
        }

        #endregion

    }
}