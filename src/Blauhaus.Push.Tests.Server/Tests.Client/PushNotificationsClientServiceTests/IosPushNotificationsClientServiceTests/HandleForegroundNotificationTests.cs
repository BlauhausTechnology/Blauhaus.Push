using System.Threading.Tasks;
using Blauhaus.Push.Client.Services;
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
        public async Task SHOULD_parse_and_publish_Async_Update()
        {
            //Arrange
            await Sut.SubscribeAsync(async notification => { PushNotificationAwaiter.SetResult(notification); });

            //Act
            Sut.HandleForegroundNotification(IosNotification);
            var result = await PushNotificationAwaiter.Task;

            //Assert
            Assert.That(result.DataProperties["message"], Is.EqualTo("This is the Message"));
            Assert.That(result.DataProperties["exclusive"], Is.EqualTo("Win!"));
            Assert.That(result.DataProperties["integer"], Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("DefaultTitle"));
            Assert.That(result.Body, Is.EqualTo("DefaultBody"));
            Assert.That(result.Name, Is.EqualTo("My Template"));
        }


     
    }
}