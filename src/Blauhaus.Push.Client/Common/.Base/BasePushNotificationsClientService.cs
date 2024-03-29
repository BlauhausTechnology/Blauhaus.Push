﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common.Notifications;

namespace Blauhaus.Push.Client.Common.Base
{
    public abstract class BasePushNotificationsClientService : BasePublisher, IPushNotificationsClientService
    {
        private const string PnsHandleKey = "PnsHandle";
        private string _currentPnsHandle;

        protected readonly IAnalyticsService AnalyticsService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly IPushNotificationTapHandler _pushNotificationTapHandler;

        protected BasePushNotificationsClientService(
            IAnalyticsService analyticsService,
            ISecureStorageService secureStorageService,
            IPushNotificationTapHandler pushNotificationTapHandler)
        {
            AnalyticsService = analyticsService;
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
                    AnalyticsService.TraceVerbose(this, "No PnsHandle found");
                }
                else
                {
                    _currentPnsHandle = storedHandle;
                    AnalyticsService.TraceVerbose(this, "PnsHandle loaded", "PnsHandle", _currentPnsHandle);
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
                    AnalyticsService.TraceInformation(this, "PnsHandle saved", "PnsHandle", pnsHandle);
                }
                else
                {
                    AnalyticsService.TraceInformation(this, "PnsHandle updated", new Dictionary<string, object>
                    {
                        { "OldPnsHandle", currentHandle },
                        { "NewPnsHandle", pnsHandle },
                    });
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
            AnalyticsService.TraceVerbose(this, "Foreground notification being published", e.NewNotification.ToObjectDictionary("PushNotification"));
            await UpdateSubscribersAsync(e.NewNotification);
        }

        #endregion
        
        
        public IObservable<IPushNotification> ObserveForegroundNotifications()
        {
            return Observable.Create<IPushNotification>(observer =>
            {

                void HandleNewNotification(object sender, NewNotificationEventArgs e)
                {
                    AnalyticsService.TraceVerbose(this, "Foreground notification being published", e.NewNotification.ToObjectDictionary("PushNotification"));
                    observer.OnNext(e.NewNotification);
                } 
                
                var newNotificationSubscription = Observable.FromEventPattern(
                    x => NewNotificationEvent += HandleNewNotification, 
                    x => NewNotificationEvent -= HandleNewNotification)
                        .Subscribe();

                return newNotificationSubscription;
            });

        }


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
            AnalyticsService.TraceVerbose(this, "BAckground notification tapped", pushNotification.ToObjectDictionary("PushNotification"));
            await _pushNotificationTapHandler.HandleTapAsync(pushNotification);
        }

        #endregion

    }
}