using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Client.Common.Services;
using Blauhaus.Push.Tests.Tests.Client._Base;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.UwpPushNotificationsClientServiceTests
{
    public class HandleAppLaunchingAsyncTests : BasePushTest<UwpPushNotificationsClientService>
    {

        private const string Arguments =
            "{Title:%22DefaultTitle%22, " +
            "Body:%22DefaultBody%22, " +
            "message:%22This is the Message%22, " +
            "exclusive:%22Win!%22, " +
            "Template_Name:%22My Template%22, " +
            "integer:%221%22" +
            "}";
            
        [Test]
        public async Task SHOULD_parse_Standard_and_DataProperties_when_invoking_handler()
        {
            //Act
            await Sut.HandleAppLaunchingAsync(Arguments);

            //Assert
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => y.Title == "DefaultTitle")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => y.Body == "DefaultBody")));
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.Is<IPushNotification>(y => y.Name == "My Template")));
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
        public async Task IF_Arguments_is_empty_SHOULD_do_nothing()
        {
            //Act
            await Sut.HandleAppLaunchingAsync(string.Empty);

            //Assert
            MockPushNotificationTapHandler.Mock.Verify(x => x.HandleTapAsync(It.IsAny<IPushNotification>()), Times.Never); 
        }

  
    }
}