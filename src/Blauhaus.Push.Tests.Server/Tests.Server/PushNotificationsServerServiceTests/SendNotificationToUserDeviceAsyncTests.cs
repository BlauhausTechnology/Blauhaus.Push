﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Common.Templates._Base;
using Blauhaus.Push.Server.Service;
using Blauhaus.Push.Tests.Tests.Server._Base;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Server.PushNotificationsServerServiceTests
{
    public class SendNotificationToUserDeviceAsyncTests : BasePushNotificationsServerTest<PushNotificationsServerService>
    {
        private PushNotificationTemplate _pushNotificationTemplate;
        private string _userId;
        private IPushNotification _notification;
        private string _deviceIdentifier;


        public override void SetUp()
        {
            base.SetUp();

            _userId = Guid.NewGuid().ToString();
            _deviceIdentifier = Guid.NewGuid().ToString();

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
            await Sut.SendNotificationToUserDeviceAsync(_notification, _userId, _deviceIdentifier, MockNotificationHub.Object);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.Initialize(MockNotificationHub.Object));
        }

        [Test]
        public async Task SHOULD_track_operation()
        {
            //Act
            await Sut.SendNotificationToUserDeviceAsync(_notification, _userId, _deviceIdentifier, MockNotificationHub.Object);

            //Assert
            MockAnalyticsService.VerifyTrace("Sending push notification to user device");
            MockAnalyticsService.VerifyTraceProperty(nameof(IPushNotification), _notification);
            MockAnalyticsService.VerifyTraceProperty("UserId", _userId);
            MockAnalyticsService.VerifyTraceProperty("DeviceIdentifier", _deviceIdentifier);
        }

        [Test]
        public async Task SHOULD_compose_tags_into_AND_clause_and_invoke_send_on_hub_client()
        {
            //Act
            await Sut.SendNotificationToUserDeviceAsync(_notification, _userId, _deviceIdentifier, MockNotificationHub.Object);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.SendNotificationAsync(
                It.Is<Dictionary<string, string>>(y => 
                    y["PropertyOne"] == "ValueOne" &&
                    y["PropertyTwo"] == "ValueTwo"), 
                It.Is<List<string>>(z => 
                    z.Count == 1 &&
                    z[0] == $"(UserId_{_userId} && MyTemplate && DeviceIdentifier_{_deviceIdentifier})")));
        }


    }
}