using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Server.Service;
using Blauhaus.Push.Tests.Server.Tests._Base;
using Microsoft.Azure.NotificationHubs;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Server.Tests.PushNotificationsServerServiceTests
{
    [TestFixture]
    public class LoadRegistrationForUserDeviceAsyncTests : BasePushNotificationsServerTest<PushNotificationsServerService>
    {
        private Installation _installation;


        public override void SetUp()
        {
            base.SetUp();

            _installation = new Installation
            {
                InstallationId = "myUserId___myDeviceId",
                Platform = NotificationPlatform.Fcm,
                ExpirationTime = null,
                PushChannel = "myPnsHandle",
                PushChannelExpired = false,
                PushVariables = null,
                Tags = new List<string> {"TagOne", "TagTwo"},
                Templates = new Dictionary<string, InstallationTemplate>
                {
                    {
                        "DummyTemplate", new InstallationTemplate
                        {
                            Body = "{ \"data\" : { \"DummyPropertyOne\" : \"$(DummyPropertyOne)\", \"DummyPropertyTwo\" : \"$(DummyPropertyTwo)\" }}"
                        }
                    }
                }
            };
            MockNotificationHubClientProxy.Mock.Setup(x => x.GetInstallationAsync(_installation.InstallationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_installation);
        }

        public class NonPlatformSpecificRegistration : LoadRegistrationForUserDeviceAsyncTests
        {
            [Test]
            public async Task SHOULD_track_operation()
            {
                //Act
                await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                MockAnalyticsService.VerifyContinueOperation("Load push notification registration for user device");
                MockAnalyticsService.VerifyContinueOperationProperty("UserId", "myUserId");
                MockAnalyticsService.VerifyContinueOperationProperty("DeviceIdentifier", "myDeviceId");
            }

            [Test]
            public async Task SHOULD_initialize_client()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.Initialize(MockNotificationHub.Object));
            }

            [Test]
            public async Task IF_installation_does_not_exist_SHOULD_fail()
            {
                //Arrange
                MockNotificationHubClientProxy.Mock.Setup(x => x.InstallationExistsAsync(_installation.InstallationId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.AreEqual(PushErrors.RegistrationDoesNotExist.ToString(), result.Error);
                MockAnalyticsService.VerifyTrace(PushErrors.RegistrationDoesNotExist.Code, LogSeverity.Error);
            }

            [Test]
            public async Task SHOULD_convert_InstallationId_to_DeviceIdentifier()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.AreEqual( "myDeviceId", result.Value.DeviceIdentifier);
            }

            [Test]
            public async Task IF_there_is_a_UserId_tag_SHOULD_convert_to_UserId()
            {
                //Arrange
                _installation.Tags.Add("UserId_MyUser");

                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.AreEqual("MyUser", result.Value.UserId);
            }

            [Test]
            public async Task IF_there_is_no_UserId_tag_SHOULD_set_UserId_empty()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.AreEqual("", result.Value.UserId);
            }

            [Test]
            public async Task IF_there_is_an_AccountId_tag_SHOULD_convert_to_AccountId()
            {
                //Arrange
                _installation.Tags.Add("AccountId_MyAccount");

                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.AreEqual("MyAccount", result.Value.AccountId);
            }

            [Test]
            public async Task IF_there_is_no_AccountId_tag_SHOULD_set_AccountId_empty()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.AreEqual(string.Empty, result.Value.AccountId);
            }

            [Test]
            public async Task SHOULD_extract_PnsHandle()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.AreEqual("myPnsHandle", result.Value.PushNotificationServiceHandle);
            }

            [Test]
            public async Task SHOULD_extract_Tags()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.True(result.Value.Tags.Contains("TagOne"));
                Assert.True(result.Value.Tags.Contains("TagTwo"));
                Assert.AreEqual(2, result.Value.Tags.Count);
            }
        }

        public class LoadIosRegistration : LoadRegistrationForUserDeviceAsyncTests
        {
            public override void SetUp()
            {
                base.SetUp();
                _installation.Platform = NotificationPlatform.Apns;
                _installation.Templates = new Dictionary<string, InstallationTemplate>
                {
                    {
                        "DummyTemplate", new InstallationTemplate
                        {
                            Body = "{ \"aps\" : { \"alert\" : { \"title\" : \"$(Title)\", \"body\" : \"$(Body)\" } }, \"VisibleTemplateProperty\" : \"$(VisibleTemplateProperty)\" }"
                        }
                    }
                };
            }

            [Test]
            public async Task WHEN_Platform_is_APNS_SHOULD_return_platform_IOS()
            {
                //Arrange
                _installation.Platform = NotificationPlatform.Apns;

                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.AreEqual(result.Value.Platform, RuntimePlatform.iOS);
            }
            
            [Test]
            public async Task SHOULD_Extract_Templates()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                var template = result.Value.Templates.First();
                Assert.AreEqual("DummyTemplate", template.NotificationName);
                Assert.AreEqual(1, template.DataProperties.Count);
                Assert.That(template.DataProperties.Contains("VisibleTemplateProperty"));
            }

        }

        public class LoadAndroidRegistration : LoadRegistrationForUserDeviceAsyncTests
        {
            [Test]
            public async Task IF_Platform_is_FCM_SHOULD_Extract_Templates()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                var template = result.Value.Templates.First();
                Assert.AreEqual("DummyTemplate", template.NotificationName);
                Assert.AreEqual(2, template.DataProperties.Count);
                Assert.That(template.DataProperties.Contains("DummyPropertyOne"));
                Assert.That(template.DataProperties.Contains("DummyPropertyTwo"));
            }

            [Test]
            public async Task WHEN_Platform_is_FCM_SHOULD_return_platform_Android()
            {
                //Arrange
                _installation.Platform = NotificationPlatform.Fcm;

                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.AreEqual(result.Value.Platform, RuntimePlatform.Android);
            }
        }

        public class LoadUwpRegistration : LoadRegistrationForUserDeviceAsyncTests
        {
            public override void SetUp()
            {
                base.SetUp();
                _installation.Platform = NotificationPlatform.Wns;
                _installation.Templates = new Dictionary<string, InstallationTemplate>
                {
                    {
                        "Visible", new InstallationTemplate
                        {
                            Body = "<toast launch=\"" +
                                   "{'{' + " +
                                   "'Title' + ':' + '%22' + $(Title) + '%22' + ', ' + " +
                                   "'Body' + ':' + '%22' + $(Body) + '%22' + ', ' + " +
                                   "'message' + ':' + '%22' + $(message) + '%22' + ', ' + " +
                                   "'exclusive' + ':' + '%22' + $(exclusive) + '%22' + ', ' + " +
                                   "'integer' + ':' + '%22' + $(integer) + '%22' +" +
                                   " '}'}" +
                                   "\"><visual><binding template=\"ToastText01\"><text id=\"1\">$(Title)</text><text id=\"2\">$(Body)</text></binding></visual></toast>"
                        }
                    }
                };

            }

            [Test]
            public async Task WHEN_Platform_is_WNS_SHOULD_return_platform_UWP()
            {
                //Arrange
                _installation.Platform = NotificationPlatform.Wns;

                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                Assert.AreEqual(result.Value.Platform, RuntimePlatform.UWP);
            }
            
            [Test]
            public async Task SHOULD_Extract_Templates()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken.None);

                //Assert
                var template = result.Value.Templates.First();
                Assert.AreEqual("Visible", template.NotificationName);
                Assert.AreEqual(3, template.DataProperties.Count);
                Assert.That(template.DataProperties.Contains("message"));
                Assert.That(template.DataProperties.Contains("exclusive"));
                Assert.That(template.DataProperties.Contains("integer"));
                Assert.That(template.DataProperties.Count, Is.EqualTo(3));
            }

        }
    }
}