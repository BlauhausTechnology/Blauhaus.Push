using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Push.Client.Common.Services;
using Blauhaus.Push.Tests.Tests.Client._Base;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.AndroidPushNotificationsClientServiceTests
{
    public class HandleForegroundNotificationTests : BasePushTest<AndroidPushNotificationsClientService>
    {
        private static readonly Dictionary<string, object> AndroidMessageProperties = new Dictionary<string, object>
        {
            {"integer", "1" },
            {"exclusive", "Win!" },
            {"message", "This is the Message" },
            {"Title", "DefaultTitle" },
            {"Body", "DefaultBody" },
            {"Template_Name", "My Template Name" },
        };

        [Test]
        public async Task SHOULD_parse_and_publish_Notification()
        {
            //Arrange
            Sut.ObserveForegroundNotifications()
                .Subscribe(notification => { PushNotificationAwaiter.SetResult(notification); });

            //Act
            Sut.HandleForegroundNotification(AndroidMessageProperties);
            var result = await PushNotificationAwaiter.Task;

            //Assert
            Assert.AreEqual("This is the Message", result.DataProperties["message"]);
            Assert.AreEqual("Win!", result.DataProperties["exclusive"]);
            Assert.AreEqual(1, result.DataProperties["integer"]);
            Assert.AreEqual("DefaultTitle", result.Title);
            Assert.AreEqual("DefaultBody", result.Body);
            Assert.AreEqual("My Template Name", result.Name);
        }


        [Test]
        public void SHOULD_log_operation_and_trace_content()
        {
            //Test
            Sut.HandleForegroundNotification(AndroidMessageProperties);

            //Assert
            MockAnalyticsService.VerifyStartTrace("Foreground Push Notification");
            MockAnalyticsService.VerifyTrace("Extracting push notification");
            MockAnalyticsService.VerifyTraceProperty("Raw Notification", AndroidMessageProperties);
            MockAnalyticsService.VerifyTrace("Notification processed");
        }

        [Test]
        public void SHOULD_trace_number_of_subscribers()
        {
            //Arrange
            Sut.ObserveForegroundNotifications().Subscribe();

            //Test
            Sut.HandleForegroundNotification(AndroidMessageProperties);

            //Assert
            MockAnalyticsService.VerifyStartTrace("Foreground Push Notification");
            MockAnalyticsService.VerifyTrace("Extracting push notification");
            MockAnalyticsService.VerifyTraceProperty("Raw Notification", AndroidMessageProperties);
            MockAnalyticsService.VerifyTrace("Notification processed");
            MockAnalyticsService.VerifyTrace("Foreground notification being published");
        }

        [Test]
        public void IF_exception_is_thrown_SHOULD_log()
        {
            //Act
            Sut.HandleForegroundNotification(null);

            //Assert
            MockAnalyticsService.VerifyLogException<ArgumentNullException>();
        }
    }
}