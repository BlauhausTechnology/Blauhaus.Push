﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Client.Common.Services;
using Blauhaus.Push.Tests.Tests.Client._Base;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.AndroidPushNotificationsClientServiceTests
{
    public class HandleNotificationTappedAsyncTests : BasePushTest<AndroidPushNotificationsClientService>
    {
        private static readonly Dictionary<string, object> AndroidIntentProperties = new Dictionary<string, object>
            {
                {"google.delivered_priority", "ignore" },
                {"google.sent_time", "ignore" },
                {"google.ttl", "ignore" },
                {"google.original_priority", "ignore" },
                {"from", "ignore" },
                {"google.message_id", "ignore" },
                {"collapse_key", "ignore" },

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
            //Act
            await Sut.HandleNotificationTappedAsync(AndroidIntentProperties);

            //Assert
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => y.Title == "DefaultTitle")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => y.Body == "DefaultBody")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => y.Name == "My Template Name")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => 
                (string) y.DataProperties["message"] == "This is the Message")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => 
                (string) y.DataProperties["exclusive"] == "Win!")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => 
                (int) y.DataProperties["integer"] == 1)));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => 
                y.DataProperties.Count == 3)));
        }

        [Test]
        public async Task IF_properties_does_not_include_Template_Name_SHOULD_ignore_but_log()
        {
            //Arrange
            var fakeNotification = new Dictionary<string, object>
            {
                {"profile", 0 }
            };

            //Act
            await Sut.HandleNotificationTappedAsync(fakeNotification);

            //Assert
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.IsAny<IPushNotification>()), Times.Never);
            MockAnalyticsService.VerifyTrace("Payload is not a notification: ignoring");
            MockAnalyticsService.VerifyTraceProperty("Raw Notification", fakeNotification);
        }


        [Test]
        public async Task SHOULD_log_operation_and_trace_content()
        {
            //Test
            await Sut.HandleNotificationTappedAsync(AndroidIntentProperties);

            //Assert
            MockAnalyticsService.VerifyStartTrace("Push Notification Tapped");
            MockAnalyticsService.VerifyTrace("Extracting push notification");
            MockAnalyticsService.VerifyTraceProperty("Raw Notification", AndroidIntentProperties);
            MockAnalyticsService.VerifyTrace("Notification processed");
        }

        [Test]
        public async Task IF_exception_is_thrown_SHOULD_log()
        {
            //Arrange
            MockPushNotificationTapHandler.Mock.Setup(x => x.HandleTapAsync(It.IsAny<IPushNotification>()))
                .ThrowsAsync(new ArgumentException("oh no you don't"));

            //Act
            await Sut.HandleNotificationTappedAsync(AndroidIntentProperties);

            //Assert
            MockAnalyticsService.VerifyLogException<ArgumentException>("oh no you don't");
        }
    }
}