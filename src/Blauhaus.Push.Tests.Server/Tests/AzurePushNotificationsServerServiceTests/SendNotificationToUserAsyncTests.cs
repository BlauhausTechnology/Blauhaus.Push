using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions;
using Blauhaus.Push.Server.Notifications;
using Blauhaus.Push.Server.Service;
using Blauhaus.Push.Server.Templates;
using Blauhaus.Push.Tests.Server.Tests._Base;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Server.Tests.AzurePushNotificationsServerServiceTests
{
    public class SendNotificationToUserAsyncTests : BasePushNotificationsServerTest<AzurePushNotificationsServerService>
    {
        private NotificationTemplate _notificationTemplate;
        private string _userId;
        private IPushNotification _notification;


        public override void SetUp()
        {
            base.SetUp();
            _userId = Guid.NewGuid().ToString();
            _notificationTemplate = new NotificationTemplate("MyTemplate", "My Title", "My Body", new List<string>
            {
                "PropertyOne",
                "PropertyTwo"
            });
            _notification = new PushNotificationBuilder(_notificationTemplate)
                .WithDataProperty("PropertyOne", "ValueOne")
                .WithDataProperty("PropertyTwo", "ValueTwo")
                .Create();
        }

        [Test]
        public async Task SHOULD_track_operation()
        {
            //Act
            await Sut.SendNotificationToUserAsync(_notification, _userId, CancellationToken.None);

            //Assert
            MockAnalyticsService.VerifyStartOperation("Send push notification to user");
            MockAnalyticsService.VerifyStartOperationProperty(nameof(IPushNotification), _notification);
            MockAnalyticsService.VerifyStartOperationProperty("UserId", _userId);
        }


        [Test]
        public async Task SHOULD_invoke_send_on_hub_client()
        {
            //Act
            await Sut.SendNotificationToUserAsync(_notification, _userId, CancellationToken.None);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.SendNotificationAsync(
                It.Is<Dictionary<string, string>>(y => 
                    y["Template_Type"] == "MyTemplate" &&
                    y["PropertyOne"] == "ValueOne" &&
                    y["PropertyTwo"] == "ValueTwo"), 
                It.Is<List<string>>(z => 
                    z.Contains("UserId_" + _userId) && 
                    z.Contains("MyTemplate")),      
                It.IsAny<CancellationToken>()));
        }


    }
}