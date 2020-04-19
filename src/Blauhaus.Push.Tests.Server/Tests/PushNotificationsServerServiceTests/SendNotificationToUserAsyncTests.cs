using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Common.Templates._Base;
using Blauhaus.Push.Server.Service;
using Blauhaus.Push.Tests.Server.Tests._Base;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Server.Tests.PushNotificationsServerServiceTests
{
    public class SendNotificationToUserAsyncTests : BasePushNotificationsServerTest<PushNotificationsServerService>
    {
        private PushNotificationTemplate _pushNotificationTemplate;
        private string _userId;
        private IPushNotification _notification;


        public override void SetUp()
        {
            base.SetUp();
            _userId = Guid.NewGuid().ToString();
            _pushNotificationTemplate = new PushNotificationTemplate("MyTemplate", "My Title", "My Body", new List<string>
            {
                "PropertyOne",
                "PropertyTwo"
            });
            _notification = new PushNotificationBuilder(_pushNotificationTemplate)
                .WithDataProperty("PropertyOne", "ValueOne")
                .WithDataProperty("PropertyTwo", "ValueTwo")
                .Create();
        } 
        
        [Test]
        public async Task SHOULD_initialize_client()
        {
            //Act
            await Sut.SendNotificationToUserAsync(_notification, _userId, MockNotificationHub.Object, CancellationToken.None);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.Initialize(MockNotificationHub.Object));
        }

        [Test]
        public async Task SHOULD_track_operation()
        {
            //Act
            await Sut.SendNotificationToUserAsync(_notification, _userId, MockNotificationHub.Object, CancellationToken.None);

            //Assert
            MockAnalyticsService.VerifyContinueOperation("Send push notification to user");
            MockAnalyticsService.VerifyContinueOperationProperty(nameof(IPushNotification), _notification);
            MockAnalyticsService.VerifyContinueOperationProperty("UserId", _userId);
        }


        [Test]
        public async Task SHOULD_invoke_send_on_hub_client()
        {
            //Act
            await Sut.SendNotificationToUserAsync(_notification, _userId, MockNotificationHub.Object, CancellationToken.None);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.SendNotificationAsync(
                It.Is<Dictionary<string, string>>(y => 
                    y["PropertyOne"] == "ValueOne" &&
                    y["PropertyTwo"] == "ValueTwo"), 
                It.Is<List<string>>(z => 
                    z.Contains("UserId_" + _userId) && 
                    z.Contains("MyTemplate")),      
                It.IsAny<CancellationToken>()));
        }


    }
}