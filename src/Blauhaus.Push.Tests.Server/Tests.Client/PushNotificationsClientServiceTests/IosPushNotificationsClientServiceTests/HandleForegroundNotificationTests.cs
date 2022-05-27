using System;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
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
        public async Task SHOULD_parse_and_publish_Async_Update()
        {
            //Arrange
            await Sut.SubscribeAsync(async notification => { PushNotificationAwaiter.SetResult(notification); });

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


     
    }
}