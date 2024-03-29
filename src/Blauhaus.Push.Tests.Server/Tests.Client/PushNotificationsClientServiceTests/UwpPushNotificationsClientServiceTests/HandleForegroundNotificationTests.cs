﻿using System;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Client.Common.Services;
using Blauhaus.Push.Tests.Tests.Client._Base;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.UwpPushNotificationsClientServiceTests
{
    public class HandleForegroundNotificationTests : BasePushTest<UwpPushNotificationsClientService>
    {

        private const string ForegroundNotificationWithProperties =
                "<toast launch=\"{" +
                "Title:%22DefaultTitle%22, " +
                "Body:%22DefaultBody%22, " +
                "message:%22This is the Message%22, " +
                "exclusive:%22Win!%22, " +
                "Template_Name:%22My Template%22, " +
                "integer:%221%22" +
                "}\">\r\n  <visual>\r\n    <binding template=\"ToastText01\">\r\n      <text id=\"1\">DefaultTitle</text>\r\n      <text id=\"2\">DefaultBody</text>\r\n    </binding>\r\n  </visual>\r\n</toast>";

        [Test]
        public async Task SHOULD_parse_DataProperties_and_publish_Observable_Notification()
        {
            //Arrange
            Sut.ObserveForegroundNotifications().Subscribe(notification => { PushNotificationAwaiter.SetResult(notification); });

            //Act
            Sut.HandleForegroundNotification(ForegroundNotificationWithProperties);
            var result = await PushNotificationAwaiter.Task;

            //Assert
            Assert.AreEqual("DefaultTitle", result.Title);
            Assert.AreEqual("DefaultBody", result.Body);
            Assert.AreEqual("My Template", result.Name);
            Assert.AreEqual("This is the Message", result.DataProperties["message"]);
            Assert.AreEqual("Win!", result.DataProperties["exclusive"]);
            Assert.AreEqual(1, result.DataProperties["integer"]);
            Assert.AreEqual(3, result.DataProperties.Count);
        }
        
        [Test]
        public async Task SHOULD_parse_DataProperties_and_publish_AsyncPublisher_update()
        {
            //Arrange
            await Sut.SubscribeAsync(async notification => { PushNotificationAwaiter.SetResult(notification); });

            //Act
            Sut.HandleForegroundNotification(ForegroundNotificationWithProperties);
            var result = await PushNotificationAwaiter.Task;

            //Assert
            Assert.AreEqual("DefaultTitle", result.Title);
            Assert.AreEqual("DefaultBody", result.Body);
            Assert.AreEqual("My Template", result.Name);
            Assert.AreEqual("This is the Message", result.DataProperties["message"]);
            Assert.AreEqual("Win!", result.DataProperties["exclusive"]);
            Assert.AreEqual(1, result.DataProperties["integer"]);
            Assert.AreEqual(3, result.DataProperties.Count);
        }

        [Test]
        public void SHOULD_log_operation_and_trace_content()
        {
            //Test
            Sut.HandleForegroundNotification(ForegroundNotificationWithProperties);

            //Assert
            MockAnalyticsService.VerifyStartTrace("Foreground Push Notification");
            MockAnalyticsService.VerifyTrace("Extracting push notification");
            MockAnalyticsService.VerifyTraceProperty("Raw Notification", ForegroundNotificationWithProperties);
            MockAnalyticsService.VerifyTrace("Notification processed");

        }

        [Test]
        public void IF_exception_is_thrown_SHOULD_log()
        {
            //Act
            Sut.HandleForegroundNotification("Won't parse");

            //Assert
            MockAnalyticsService.VerifyLogException<ArgumentException>("Did not find expected beginning text of '<toast launch=\"'");
        }
       
    }
}