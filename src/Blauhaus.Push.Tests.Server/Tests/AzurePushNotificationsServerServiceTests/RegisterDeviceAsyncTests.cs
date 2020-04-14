using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Service;
using Blauhaus.Push.Server.Templates;
using Blauhaus.Push.Server.Templates._Base;
using Blauhaus.Push.Tests.Server.Tests._Base;
using Microsoft.Azure.NotificationHubs;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Server.Tests.AzurePushNotificationsServerServiceTests
{
    [TestFixture]
    public class RegisterDeviceAsyncTests : BasePushNotificationsServerTest<AzurePushNotificationsServerService>
    {
        private DeviceRegistration _dataOnlyDeviceRegistration;
        private DeviceRegistration _visibleTemplateDeviceRegistration;

        public override void SetUp()
        {
            base.SetUp();
            _dataOnlyDeviceRegistration = new DeviceRegistration
            {
                PushNotificationServiceHandle = "myPnsHandle",
                DeviceIdentifier = "myDeviceId",
                Platform = RuntimePlatform.iOS,
                Templates = new List<INotificationTemplate>
                {
                    new NotificationTemplate("DummyTemplate", "Dummy Title", "Dummy Body", new List<string>
                    {
                        "DummyPropertyOne",
                        "DummyPropertyTwo"
                    })
                }
            };

            _visibleTemplateDeviceRegistration = new DeviceRegistration
            {
                PushNotificationServiceHandle = "myPnsHandle",
                DeviceIdentifier = "myDeviceId",
                Platform = RuntimePlatform.Android,
                Templates = new List<INotificationTemplate>
                {
                    new NotificationTemplate("VisibleTemplate", "Title", "Body", new List<string>
                    {
                        "VisibleTemplateProperty",
                        "SecondVisibleTemplateProperty"
                    })
                }
            };
        }

        public class AllPlatformsDeviceRegistration : RegisterDeviceAsyncTests
        {
            [Test]
            public async Task SHOULD_track_operation()
            {
                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                MockAnalyticsService.VerifyStartOperation("Register device for push notifications");
                MockAnalyticsService.VerifyStartOperationProperty(nameof(DeviceRegistration), _dataOnlyDeviceRegistration);
            }

            [Test]
            public async Task IF_PushNotificationServiceHandle_is_not_provided_SHOULD_fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration.PushNotificationServiceHandle = string.Empty;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                Assert.AreEqual(PushErrors.InvalidPnsHandle.ToString(), result.Error);
                MockAnalyticsService.VerifyTrace(PushErrors.InvalidPnsHandle.Code, LogSeverity.Error);
                MockAnalyticsService.VerifyTraceProperty(nameof(DeviceRegistration), _dataOnlyDeviceRegistration);
            }

            [Test]
            public async Task IF_DeviceRegistration_is_null_SHOULD_fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration = null;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                Assert.AreEqual(PushErrors.InvalidDeviceRegistration.ToString(), result.Error);
                MockAnalyticsService.VerifyTrace(PushErrors.InvalidDeviceRegistration.Code, LogSeverity.Error);
            }

            [Test]
            public async Task SHOULD_invoke_hub_to_install_with_PushNotificationServiceHandle_as_PushChannel()
            {
                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.PushChannel == "myPnsHandle"), CancellationToken.None));
            }

            [Test]
            public async Task SHOULD_invoke_hub_to_install_with_DeviceIdentifier_as_InstallationId()
            {
                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.InstallationId == "myDeviceId"), CancellationToken.None));
            }

            [Test]
            public async Task IF_DeviceIdentifier_is_not_provided_SHOULD_generate_one()
            {
                //Arrange
                _dataOnlyDeviceRegistration.DeviceIdentifier = null;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.InstallationId.Length == Guid.NewGuid().ToString().Length), CancellationToken.None));
                Assert.AreEqual(Guid.NewGuid().ToString().Length, result.Value.DeviceIdentifier.Length);
                MockAnalyticsService.VerifyTrace("No DeviceIdentifier received, generating new InstallationId");
            }

            [Test]
            public async Task IF_RuntimePlatform_is_not_supported_SHOULD_fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = RuntimePlatform.DotNetCore;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                Assert.AreEqual(PushErrors.InvalidPlatform.ToString(), result.Error);
                MockAnalyticsService.VerifyTrace(PushErrors.InvalidPlatform.Code, LogSeverity.Error);
            }

            [Test]
            public async Task IF_RuntimePlatform_is_not_provided_SHOULD_fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = null;

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                Assert.AreEqual(PushErrors.InvalidPlatform.ToString(), result.Error);
                MockAnalyticsService.VerifyTrace(PushErrors.InvalidPlatform.Code, LogSeverity.Error);
            }

            [Test]
            public async Task IF_No_Templates_are_provided_SHOULD_Fail()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Templates = new List<INotificationTemplate>();

                //Act
                var result = await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                Assert.AreEqual(PushErrors.NoTemplateProvidedOnRegistration.ToString(), result.Error);
                MockAnalyticsService.VerifyTrace(PushErrors.NoTemplateProvidedOnRegistration.Code, LogSeverity.Error);
            }

            [Test]
            public async Task IF_tags_are_provided_SHOULD_add_them_to_the_registration()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Tags = new List<string> {"ThingOne", "ThingTwo"};

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Tags.Contains("ThingOne")), CancellationToken.None));
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Tags.Contains("ThingTwo")), CancellationToken.None));
            }

            [Test]
            public async Task IF_UserId_is_provided_SHOULD_add_tag()
            {
                //Arrange
                var userId = Guid.NewGuid().ToString();
                _dataOnlyDeviceRegistration.UserId = userId;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Tags.Contains($"UserId_{userId}")), CancellationToken.None));
            }

            [Test]
            public async Task IF_AccountId_is_provided_SHOULD_add_tag()
            {
                //Arrange
                var accountId = Guid.NewGuid().ToString();
                _dataOnlyDeviceRegistration.AccountId = accountId;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Tags.Contains($"AccountId_{accountId}")), CancellationToken.None));
            }
        }

        public class UwpDeviceRegistration : RegisterDeviceAsyncTests
        {
            [Test]
            public async Task SHOULD_convert_UWP_RuntimePlatform_to_WNS_push_notification_service()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = RuntimePlatform.UWP;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Platform == NotificationPlatform.Wns), CancellationToken.None));
            }

            [Test]
            public async Task SHOULD_convert_VisibleTemplate_Properties_to_required_format()
            {
                //Arrange
                _visibleTemplateDeviceRegistration.Platform = RuntimePlatform.UWP;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_visibleTemplateDeviceRegistration, CancellationToken.None);

                //Assert
                var expectedTemplate =
                    "<toast launch=\"{'{' + " +
                    "'Title' + ':' + '%22' + $(Title) + '%22' + ', ' + " +
                    "'Body' + ':' + '%22' + $(Body) + '%22' + ', ' + " +
                    "'VisibleTemplateProperty' + ':' + '%22' + $(VisibleTemplateProperty) + '%22' + ', ' + " +
                    "'SecondVisibleTemplateProperty' + ':' + '%22' + $(SecondVisibleTemplateProperty) + '%22' + '}'}\">" +
                    "<visual><binding template=\"ToastText01\">" +
                    "<text id=\"1\">$(Title)</text>" +
                    "<text id=\"2\">$(Body)</text></binding></visual></toast>";
                
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Templates["VisibleTemplate"].Body == expectedTemplate), CancellationToken.None));
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
                    var result = await Sut.UpdateDeviceRegistrationAsync(_visibleTemplateDeviceRegistration, CancellationToken.None);
                    
                    //Assert
                    Assert.AreEqual(PushErrors.ReservedString(forbiddenString).ToString(), result.Error);
                    MockAnalyticsService.VerifyTrace(PushErrors.ReservedString(forbiddenString).Code, LogSeverity.Error);  
                    
                    //Cleanup
                    _visibleTemplateDeviceRegistration.Templates.First().DataProperties.Remove(forbiddenString);    
                }
            }

        }
        
        public class IosDeviceRegistration : RegisterDeviceAsyncTests
        {
            [Test]
            public async Task SHOULD_convert_iOS_RuntimePlatform_to_APNS_push_notification_service()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = RuntimePlatform.iOS;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Platform == NotificationPlatform.Apns), CancellationToken.None));
            }
            
            [Test]
            public async Task SHOULD_convert_VisibleTemplate_Properties_to_required_format()
            {
                //Arrange
                _visibleTemplateDeviceRegistration.Platform = RuntimePlatform.iOS;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_visibleTemplateDeviceRegistration, CancellationToken.None);

                //Assert
                const string expectedTemplate = 
                    "{ \"aps\" : { \"alert\" : { \"title\" : \"$(Title)\", \"body\" : \"$(Body)\" } }, " +
                    "\"VisibleTemplateProperty\" : \"$(VisibleTemplateProperty)\", " +
                    "\"SecondVisibleTemplateProperty\" : \"$(SecondVisibleTemplateProperty)\" }";
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Templates["VisibleTemplate"].Body == expectedTemplate), CancellationToken.None));
            }


        }

        public class AndroidDeviceRegistration : RegisterDeviceAsyncTests
        {
            [Test]
            public async Task SHOULD_convert_Android_RuntimePlatform_to_FCM_push_notification_service()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = RuntimePlatform.Android;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_dataOnlyDeviceRegistration, CancellationToken.None);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Platform == NotificationPlatform.Fcm), CancellationToken.None));
            }

            [Test]
            public async Task IF_template_is_visible_SHOULD_convert_Properties_to_required_format()
            {
                //Arrange
                _dataOnlyDeviceRegistration.Platform = RuntimePlatform.Android;

                //Act
                await Sut.UpdateDeviceRegistrationAsync(_visibleTemplateDeviceRegistration, CancellationToken.None);

                //Assert
                const string expectedTemplate = 
                    "{ \"data\" : { " +
                    "\"Title\" : \"$(Title)\", " +
                    "\"Body\" : \"$(Body)\", " +
                    "\"VisibleTemplateProperty\" : \"$(VisibleTemplateProperty)\", " +
                    "\"SecondVisibleTemplateProperty\" : \"$(SecondVisibleTemplateProperty)\" " +
                    "}, \"notification\" : { \"title\" : \"$(Title)\", \"body\" : \"$(Body)\" } }";
                MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y =>
                    y.Templates["VisibleTemplate"].Body == expectedTemplate), CancellationToken.None));
            }
        }
    }
}