﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers.Extensions;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Templates._Base;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Service;
using Blauhaus.Push.Tests.Tests.Server._Base;
using Microsoft.Azure.NotificationHubs;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Server.PushNotificationsServerServiceTests
{
    [TestFixture]
    public class UpdateDeviceRegistrationAsyncTests : BasePushNotificationsServerTest<PushNotificationsServerService>
    {
        private DeviceRegistration _dataOnlyDeviceRegistration;
        private DeviceRegistration _visibleTemplateDeviceRegistration;

        public override void SetUp()
        {
            base.SetUp();
            _dataOnlyDeviceRegistration = new DeviceRegistration
            {
                UserId = "myUserId",
                PushNotificationServiceHandle = "myPnsHandle",
                DeviceIdentifier = "myDeviceId",
                Platform = RuntimePlatform.iOS,
                Templates = new List<IPushNotificationTemplate>
                {
                    new PushNotificationTemplate("DummyTemplate", "Dummy Title", "Dummy Body", new List<string>
                    {
                        "DummyPropertyOne",
                        "DummyPropertyTwo"
                    })
                }
            };

            _visibleTemplateDeviceRegistration = new DeviceRegistration
            {
                UserId = "myUserId",
                PushNotificationServiceHandle = "myPnsHandle",
                DeviceIdentifier = "myDeviceId",
                Platform = RuntimePlatform.Android,
                Templates = new List<IPushNotificationTemplate>
                {
                    new PushNotificationTemplate("VisibleTemplate", "Title", "Body", new List<string>
                    {
                        "VisibleTemplateProperty",
                        "SecondVisibleTemplateProperty"
                    })
                }
            };
        }

        [Test]
        public async Task SHOULD_initialize_client()
        {
            //Act
            var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.Initialize(MockNotificationHub.Object));
        }

        public class AllPlatformsDeviceRegistration : UpdateDeviceRegistrationAsyncTests
        {
            [Test]
            public async Task SHOULD_track_operation()
            {
                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockAnalyticsService.VerifyTrace("Register device for push notifications");
                MockAnalyticsService.VerifyTraceProperty(nameof(DeviceRegistration), _dataOnlyDeviceRegistration);
            }

            [Test]
            public async Task IF_PushNotificationServiceHandle_is_not_provided_SHOULD_fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration.PushNotificationServiceHandle = string.Empty;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                result.VerifyResponseError(PushErrors.InvalidPnsHandle, MockAnalyticsService); 
            }

            [Test]
            public async Task IF_DeviceRegistration_is_null_SHOULD_fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration = null;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                result.VerifyResponseError(PushErrors.InvalidDeviceRegistration, MockAnalyticsService);  
            }

            [Test]
            public async Task SHOULD_invoke_hub_to_install_with_PushNotificationServiceHandle_as_PushChannel()
            {
                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.PushChannel == "myPnsHandle")));
            }

            [Test]
            public async Task SHOULD_invoke_hub_to_install_with_DeviceIdentifier_and_userId_as_InstallationId()
            {
                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.InstallationId == "myUserId" + "___" + "myDeviceId")));
            }

            [Test]
            public async Task IF_DeviceIdentifier_is_not_provided_SHOULD_fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration.DeviceIdentifier = null;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                result.VerifyResponseError(PushErrors.MissingDeviceIdentifier, MockAnalyticsService); 
            }

            [Test]
            public async Task IF_UserId_is_not_provided_SHOULD_fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration.UserId = null;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                result.VerifyResponseError(PushErrors.MissingUserId, MockAnalyticsService); 
            }

            [Test]
            public async Task IF_RuntimePlatform_is_not_supported_SHOULD_fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = RuntimePlatform.DotNetCore;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                result.VerifyResponseError(PushErrors.InvalidPlatform, MockAnalyticsService);  
            }

            [Test]
            public async Task IF_RuntimePlatform_is_not_provided_SHOULD_fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = null;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                result.VerifyResponseError(PushErrors.InvalidPlatform, MockAnalyticsService);  
            }

            [Test]
            public async Task IF_No_Templates_are_provided_SHOULD_send_update_with_empty_list_to_clear_subscriptions()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Templates = new List<IPushNotificationTemplate>();

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Templates.Count==0)));
            }

            [Test]
            public async Task IF_tags_are_provided_SHOULD_add_them_to_the_registration()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Tags = new List<string> {"ThingOne", "ThingTwo"};

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Tags.Contains("ThingOne"))));
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Tags.Contains("ThingTwo"))));
            }

            [Test]
            public async Task SHOULD_add_UserId_tag()
            {
                //Arrange
                var userId = Guid.NewGuid().ToString();
                _dataOnlyDeviceRegistration.UserId = userId;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Tags.Contains($"UserId_{userId}"))));
            }
            
            [Test]
            public async Task SHOULD_add_DeviceIdentifier_tag()
            {
                //Arrange
                var deviceId = Guid.NewGuid().ToString();
                _dataOnlyDeviceRegistration.DeviceIdentifier = deviceId;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Tags.Contains($"DeviceIdentifier_{deviceId}"))));
            }

            [Test]
            public async Task IF_AccountId_is_provided_SHOULD_add_tag()
            {
                //Arrange
                var accountId = Guid.NewGuid().ToString();
                _dataOnlyDeviceRegistration.AccountId = accountId;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Tags.Contains($"AccountId_{accountId}"))));
            }

            [Test]
            public async Task SHOULD_trace()
            {
                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockAnalyticsService.VerifyTrace("Push notification registration updated");
            }
        }

        public class UwpDeviceRegistration : UpdateDeviceRegistrationAsyncTests
        {
            [Test]
            public async Task SHOULD_convert_UWP_RuntimePlatform_to_WNS_push_notification_service()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = RuntimePlatform.UWP;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Platform == NotificationPlatform.Wns)));
            }

            [Test]
            public async Task SHOULD_convert_VisibleTemplate_Properties_to_required_format()
            {
                //Arrange
                _visibleTemplateDeviceRegistration.Platform = RuntimePlatform.UWP;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_visibleTemplateDeviceRegistration, MockNotificationHub.Object);

                //Assert
                var expectedTemplate =
                    "<toast launch=\"{'{' + " +
                    "'Title' + ':' + '%22' + $(Title) + '%22' + ', ' + " +
                    "'Body' + ':' + '%22' + $(Body) + '%22' + ', ' + " +
                    "'Template_Name' + ':' + '%22' + 'VisibleTemplate' + '%22' + ', ' + " +
                    "'VisibleTemplateProperty' + ':' + '%22' + $(VisibleTemplateProperty) + '%22' + ', ' + " +
                    "'SecondVisibleTemplateProperty' + ':' + '%22' + $(SecondVisibleTemplateProperty) + '%22' + '}'}\">" +
                    "<visual><binding template=\"ToastText01\">" +
                    "<text id=\"1\">$(Title)</text>" +
                    "<text id=\"2\">$(Body)</text></binding></visual></toast>";
                
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Templates["VisibleTemplate"].Body == expectedTemplate)));
            }

            [Test]
            public async Task IF_registration_contains_forbidden_string_SHOULD_fail()
            {
                _visibleTemplateDeviceRegistration.Platform = RuntimePlatform.UWP;

                foreach (var forbiddenString in ReservedStrings.Uwp)
                {
                    //Arrange
                    _visibleTemplateDeviceRegistration.Templates.First().DataProperties.Add(forbiddenString);
                    
                    //Act
                    var result = await Sut.UpdateDeviceRegistrationAsync(_visibleTemplateDeviceRegistration, MockNotificationHub.Object);
                    
                    //Assert
                    result.VerifyResponseError(PushErrors.ReservedString(forbiddenString), MockAnalyticsService);  
                    
                    //Cleanup
                    _visibleTemplateDeviceRegistration.Templates.First().DataProperties.Remove(forbiddenString);    
                }
            }

        }
        
        public class IosDeviceRegistration : UpdateDeviceRegistrationAsyncTests
        {
            [Test]
            public async Task SHOULD_convert_iOS_RuntimePlatform_to_APNS_push_notification_service()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = RuntimePlatform.iOS;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Platform == NotificationPlatform.Apns)));
            }
            
            [Test]
            public async Task SHOULD_convert_VisibleTemplate_Properties_to_required_format()
            {
                //Arrange
                _visibleTemplateDeviceRegistration.Platform = RuntimePlatform.iOS;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_visibleTemplateDeviceRegistration, MockNotificationHub.Object);

                //Assert
                const string expectedTemplate = 
                    "{ \"aps\" : { \"alert\" : { \"title\" : \"$(Title)\", \"body\" : \"$(Body)\" } }, " +
                    "\"Template_Name\" : \"VisibleTemplate\", " +
                    "\"VisibleTemplateProperty\" : \"$(VisibleTemplateProperty)\", " +
                    "\"SecondVisibleTemplateProperty\" : \"$(SecondVisibleTemplateProperty)\" }";
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Templates["VisibleTemplate"].Body == expectedTemplate)));
            }


        }

        public class AndroidDeviceRegistration : UpdateDeviceRegistrationAsyncTests
        {
            [Test]
            public async Task SHOULD_convert_Android_RuntimePlatform_to_FCM_push_notification_service()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = RuntimePlatform.Android;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Platform == NotificationPlatform.Fcm)));
            }

            [Test]
            public async Task IF_template_is_visible_SHOULD_convert_Properties_to_required_format()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = RuntimePlatform.Android;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_visibleTemplateDeviceRegistration, MockNotificationHub.Object);

                //Assert
                const string expectedTemplate = 
                    "{ \"data\" : { " +
                    "\"Title\" : \"$(Title)\", " +
                    "\"Body\" : \"$(Body)\", " +
                    "\"Template_Name\" : \"VisibleTemplate\", " +
                    "\"VisibleTemplateProperty\" : \"$(VisibleTemplateProperty)\", " +
                    "\"SecondVisibleTemplateProperty\" : \"$(SecondVisibleTemplateProperty)\" " +
                    "}, \"notification\" : { \"title\" : \"$(Title)\", \"body\" : \"$(Body)\" } }";
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Templates["VisibleTemplate"].Body == expectedTemplate)));
            }
        }
    }
}