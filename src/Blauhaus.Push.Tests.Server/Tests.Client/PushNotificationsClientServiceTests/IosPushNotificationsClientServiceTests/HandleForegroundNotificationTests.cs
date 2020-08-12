using System;
using System.Threading.Tasks;
using Blauhaus.Push.Client.Common.Services;
using Blauhaus.Push.Tests.Tests.Client._Base;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.IosPushNotificationsClientServiceTests
{
    public class HandleForegroundNotificationTests : BasePushTest<IosPushNotificationsClientService>
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
            //Arrange
            Sut.ObserveForegroundNotifications().Subscribe(notification => { PushNotificationAwaiter.SetResult(notification); });

            //Act
            Sut.HandleForegroundNotification(IosNotification);
            var result = await PushNotificationAwaiter.Task;

            //Assert
            Assert.AreEqual("This is the Message", result.DataProperties["message"]);
            Assert.AreEqual("Win!", result.DataProperties["exclusive"]);
            Assert.AreEqual(1, result.DataProperties["integer"]);
            Assert.AreEqual("DefaultTitle", result.Title);
            Assert.AreEqual("DefaultBody", result.Body);
            Assert.AreEqual("My Template", result.Name);
        }


        [Test]
        public void SHOULD_log_operation_and_trace_content()
        {
            //Test
            Sut.HandleForegroundNotification(IosNotification);

            //Assert
            MockAnalyticsService.VerifyStartOperation("Foreground Push Notification");
            MockAnalyticsService.VerifyTrace("Extracting push notification");
            MockAnalyticsService.VerifyTraceProperty("Raw Notification", IosNotification);
            MockAnalyticsService.VerifyTrace("Notification processed");
        }

        [Test]
        public void IF_exception_is_thrown_SHOULD_log()
        {
            //Act
            Sut.HandleForegroundNotification("Won't parse");

            //Assert
            MockAnalyticsService.VerifyLogException<Exception>("Unexpected character encountered while parsing value: W. Path '', line 0, position 0.");
        }
    }
}