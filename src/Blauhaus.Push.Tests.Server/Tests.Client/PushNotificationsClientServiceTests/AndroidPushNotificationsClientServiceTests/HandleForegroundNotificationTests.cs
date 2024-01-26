using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Push.Client.Services;
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
        public async Task SHOULD_parse_and_publish_Async_Notification()
        {
            //Arrange
            await Sut.SubscribeAsync(async notification => { PushNotificationAwaiter.SetResult(notification); });

            //Act
            Sut.HandleForegroundNotification(AndroidMessageProperties);
            var result = await PushNotificationAwaiter.Task;

            //Assert
            Assert.That(result.DataProperties["message"], Is.EqualTo("This is the Message"));
            Assert.That(result.DataProperties["exclusive"], Is.EqualTo("Win!"));
            Assert.That(result.DataProperties["integer"], Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("DefaultTitle"));
            Assert.That(result.Body, Is.EqualTo("DefaultBody"));
            Assert.That(result.Name, Is.EqualTo("My Template Name"));
        }
          
    }
}