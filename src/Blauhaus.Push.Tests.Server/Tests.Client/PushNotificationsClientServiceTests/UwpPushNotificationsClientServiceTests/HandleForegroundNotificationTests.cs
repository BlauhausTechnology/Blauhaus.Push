using System.Threading.Tasks;
using Blauhaus.Push.Client.Services;
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
        public async Task SHOULD_parse_DataProperties_and_publish_AsyncPublisher_update()
        {
            //Arrange
            await Sut.SubscribeAsync(async notification => { PushNotificationAwaiter.SetResult(notification); });

            //Act
            Sut.HandleForegroundNotification(ForegroundNotificationWithProperties);
            var result = await PushNotificationAwaiter.Task;

            //Assert
            Assert.That(result.Title, Is.EqualTo("DefaultTitle"));
            Assert.That(result.Body, Is.EqualTo("DefaultBody"));
            Assert.That(result.Name, Is.EqualTo("My Template"));
            Assert.That(result.DataProperties["message"], Is.EqualTo("This is the Message"));
            Assert.That(result.DataProperties["exclusive"], Is.EqualTo("Win!"));
            Assert.That(result.DataProperties["integer"], Is.EqualTo(1));
            Assert.That(result.DataProperties.Count, Is.EqualTo(3));
        }
         
    }
}