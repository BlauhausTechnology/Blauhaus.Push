using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.DeviceServices.TestHelpers.MockBuilders;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Client.Common.Services;
using Blauhaus.Push.TestHelpers.MockBuilders;
using Blauhaus.TestHelpers.BaseTests;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Client.Tests._Base
{
    public abstract class BasePushTest<TSut> : BaseServiceTest<TSut> where TSut : class
    {

        protected CancellationTokenSource CancellationTokenSource;
        protected CancellationToken Token;
        protected TaskCompletionSource<IPushNotification> PushNotificationAwaiter;
        protected TaskCompletionSource<Exception> ExceptionAwaiter;

        [SetUp]
        public virtual void Setup()
        {
            Cleanup();

            CancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));
            Token = CancellationTokenSource.Token;
            Token.ThrowIfCancellationRequested();
            
            PushNotificationAwaiter = new TaskCompletionSource<IPushNotification>(CancellationTokenSource);
            ExceptionAwaiter = new TaskCompletionSource<Exception>(CancellationTokenSource);

            Services.AddSingleton(x => MockSecureStorageService.Object);
            Services.AddSingleton(x => MockAnalyticsService.Object);
            Services.AddSingleton(x => MockPushNotificationTapHandler.Object);

            Services.AddSingleton<UwpPushNotificationsClientService>();
            Services.AddSingleton<IosPushNotificationsClientService>();
            Services.AddSingleton<AndroidPushNotificationsClientService>();
        }

        protected SecureStorageMockBuilder MockSecureStorageService => Mocks.AddMock<SecureStorageMockBuilder, ISecureStorageService>().Invoke();
        protected AnalyticsServiceMockBuilder MockAnalyticsService => Mocks.AddMock<AnalyticsServiceMockBuilder, IAnalyticsService>().Invoke();
        protected PushNotificationTapHandlerMockBuilder MockPushNotificationTapHandler => Mocks.AddMock<PushNotificationTapHandlerMockBuilder, IPushNotificationTapHandler>().Invoke();
    }
}