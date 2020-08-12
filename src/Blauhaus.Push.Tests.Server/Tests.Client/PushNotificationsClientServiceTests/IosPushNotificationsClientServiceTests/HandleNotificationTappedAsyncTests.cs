using System;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Client.Common.Services;
using Blauhaus.Push.Tests.Tests.Client._Base;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.IosPushNotificationsClientServiceTests
{
    public class HandleNotificationTappedAsyncTests : BasePushTest<IosPushNotificationsClientService>
    {
        private const string IosNotification =
            "{\"exclusive\":\"Win!\"," +
            "\"message\":\"This is the Message\"," +
            "\"integer\":\"1\"," +
            "\"Template_Name\":\"My Template\"" +
            ",\"aps\":{\"alert\":{\"title\":\"DefaultTitle\",\"body\":\"DefaultBody\"}}}";
        
        [Test]
        public async Task SHOULD_parse_and_publish_Notification()
        {
            //Act
            await Sut.HandleNotificationTappedAsync(IosNotification);

            //Assert
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => y.Title == "DefaultTitle")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => y.Body == "DefaultBody")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => y.Name == "My Template")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => 
                (string) y.DataProperties["message"] == "This is the Message")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => 
                (string) y.DataProperties["exclusive"] == "Win!")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => 
                (int) y.DataProperties["integer"] == 1)));
        }


        [Test]
        public async Task SHOULD_log_operation_and_trace_content()
        {
            //Test
            await Sut.HandleNotificationTappedAsync(IosNotification);

            //Assert
            MockAnalyticsService.VerifyStartOperation("Push Notification Tapped");
            MockAnalyticsService.VerifyTrace("Extracting push notification");
            MockAnalyticsService.VerifyTraceProperty("Raw Notification", IosNotification);
            MockAnalyticsService.VerifyTrace("Notification processed");
        }

        [Test]
        public async Task IF_exception_is_thrown_SHOULD_log()
        {
            //Arrange
            MockPushNotificationTapHandler.Mock.Setup(x => x.HandleTapAsync(It.IsAny<IPushNotification>()))
                .ThrowsAsync(new ArgumentException("oh no you don't"));

            //Act
            await Sut.HandleNotificationTappedAsync(IosNotification);

            //Assert
            MockAnalyticsService.VerifyLogException<ArgumentException>("oh no you don't");
        }
    }
}