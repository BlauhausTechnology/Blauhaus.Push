using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common.Notifications;

namespace Blauhaus.Push.Client
{
    public class LocalClientPush : IPushNotificationsClientService
    {
        private const string PnsHandleKey = "PnsHandle";
        private string _currentPnsHandle;

        private static readonly List<string> IgnoredFields =new List<string>
        {
            "google.delivered_priority",
            "google.sent_time",
            "google.ttl",
            "google.delivered_priority",
            "google.original_priority",
            "google.message_id",
            "collapse_key",
            "from",
        };

        protected readonly IAnalyticsService AnalyticsService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly IPushNotificationTapHandler _pushNotificationTapHandler;

        protected LocalClientPush(
            IAnalyticsService analyticsService,
            ISecureStorageService secureStorageService,
            IPushNotificationTapHandler pushNotificationTapHandler)
        {
            AnalyticsService = analyticsService;
            _secureStorageService = secureStorageService;
            _pushNotificationTapHandler = pushNotificationTapHandler;
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

        public IObservable<IPushNotification> ObserveForegroundNotifications()
        {
            return Observable.Create<IPushNotification>(observer =>
            {

                void HandleNewNotification(object sender, NewNotificationEventArgs e)
                {
                    AnalyticsService.TraceVerbose(this, "Foreground notification being published");
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

        internal class NewNotificationEventArgs : EventArgs
        {
            public NewNotificationEventArgs(IPushNotification newNotification)
            {
                NewNotification = newNotification;
            }

            public IPushNotification NewNotification { get; }
        }

        private event EventHandler<NewNotificationEventArgs> NewNotificationEvent;
        
        protected void PublishNotification(IPushNotification notification)
        {
            NewNotificationEvent?.Invoke(this, new NewNotificationEventArgs(notification));
        }

        #endregion
        
        #region Tapped Notification
        
        protected Task InvokeTapHandlersAsync(IPushNotification pushNotification)
        {
            return _pushNotificationTapHandler.HandleTapAsync(pushNotification);
        }

        #endregion

        

        public void HandleForegroundNotification(Dictionary<string, object> androidPayload)
        {
            using (var _ = AnalyticsService.StartTrace(this, "Foreground Push Notification"))
            {
                try
                {
                    if(androidPayload ==null) throw new ArgumentNullException();
                    PublishNotification(ExtractNotification(androidPayload));
                }
                catch (Exception e)
                {
                    AnalyticsService.LogException(this, e);
                }
            }
        }

        public async Task HandleNotificationTappedAsync(Dictionary<string, object>  androidPayload)
        {
            using (var _ = AnalyticsService.StartTrace(this, "Push Notification Tapped"))
            {
                try
                {
                    if(androidPayload ==null) throw new ArgumentNullException();
                    await InvokeTapHandlersAsync(ExtractNotification(androidPayload));
                }
                catch (Exception e)
                {
                    AnalyticsService.LogException(this, e);
                }
            }
        }

        private IPushNotification ExtractNotification(Dictionary<string, object> payload)
        {
            
            AnalyticsService.TraceVerbose(this, "Extracting push notification", new Dictionary<string, object>
            {
                {"Raw Notification", payload }
            });

            var type = "";
            var title = "";
            var body = "";
            var data = new Dictionary<string, object>();

            foreach (var notificationProperty in payload)
            {
                if (IgnoredFields.Contains(notificationProperty.Key))
                {
                    continue;
                }

                if (string.Equals(notificationProperty.Key, "title", StringComparison.InvariantCultureIgnoreCase))
                {
                    title = notificationProperty.Value.ToString();
                }
                
                else if (string.Equals(notificationProperty.Key, "body", StringComparison.InvariantCultureIgnoreCase))
                {
                    body = notificationProperty.Value.ToString();
                }

                else if (string.Equals(notificationProperty.Key, "Template_Name", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = notificationProperty.Value.ToString();
                }

                else
                {
                    if (int.TryParse(notificationProperty.Value.ToString(), out var integerValue))
                    {
                        data[notificationProperty.Key] = integerValue;
                    }
                    else
                    {
                        data[notificationProperty.Key] = notificationProperty.Value;
                    }
                }
            }

            var pushNotification = new PushNotification(type, data, title, body); 
            AnalyticsService.TraceVerbose(this, "Notification processed", pushNotification.ToObjectDictionary());

            return pushNotification;
        }
    }
}