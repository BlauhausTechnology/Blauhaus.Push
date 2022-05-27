using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Client.Common.Services;
using Blauhaus.Push.Tests.Tests.Client._Base;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.AndroidPushNotificationsClientServiceTests
{
    public class HandleForegroundNotificationTests : BasePushTest<AndroidPushNotificationsClientService>
    {
        private static readonly Dictionary<string, object> AndroidMessageProperties = new()
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
        public async Task SHOULD_parse_and_publish_Async_Notification()
        {
            //Arrange
            await Sut.SubscribeAsync(async notification => { PushNotificationAwaiter.SetResult(notification); });

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
          
    }
}