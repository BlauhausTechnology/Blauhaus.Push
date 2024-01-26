using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Server.Service;
using Blauhaus.Push.Tests.Tests.Server._Base;
using Microsoft.Azure.NotificationHubs;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Server.PushNotificationsServerServiceTests
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
            MockNotificationHubClientProxy.Mock.Setup(x => x.GetInstallationAsync(_installation.InstallationId))
                .ReturnsAsync(_installation);
        }

        public class NonPlatformSpecificRegistration : LoadRegistrationForUserDeviceAsyncTests
        {
            
            [Test]
            public async Task SHOULD_initialize_client()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                MockNotificationHubClientProxy.Mock.Verify(x => x.Initialize(MockNotificationHub.Object));
            }

            [Test]
            public async Task IF_installation_does_not_exist_SHOULD_fail()
            {
                //Arrange
                MockNotificationHubClientProxy.Mock.Setup(x => x.InstallationExistsAsync(_installation.InstallationId))
                    .ReturnsAsync(false);

                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                MockLogger.VerifyLogErrorResponse(PushErrors.RegistrationDoesNotExist, result);
            }

            [Test]
            public async Task SHOULD_convert_InstallationId_to_DeviceIdentifier()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                Assert.That(result.Value.DeviceIdentifier, Is.EqualTo("myDeviceId"));
            }

            [Test]
            public async Task IF_there_is_a_UserId_tag_SHOULD_convert_to_UserId()
            {
                //Arrange
                _installation.Tags.Add("UserId_MyUser");

                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                Assert.That(result.Value.UserId, Is.EqualTo("MyUser"));
            }

            [Test]
            public async Task IF_there_is_no_UserId_tag_SHOULD_set_UserId_empty()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                Assert.That(result.Value.UserId, Is.EqualTo(""));
            }

            [Test]
            public async Task IF_there_is_an_AccountId_tag_SHOULD_convert_to_AccountId()
            {
                //Arrange
                _installation.Tags.Add("AccountId_MyAccount");

                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                Assert.That(result.Value.AccountId, Is.EqualTo("MyAccount"));
            }

            [Test]
            public async Task IF_there_is_no_AccountId_tag_SHOULD_set_AccountId_empty()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                Assert.That(result.Value.AccountId, Is.EqualTo(string.Empty));
            }

            [Test]
            public async Task SHOULD_extract_PnsHandle()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                Assert.That(result.Value.PushNotificationServiceHandle, Is.EqualTo("myPnsHandle"));
            }

            [Test]
            public async Task SHOULD_extract_Tags()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                Assert.That(result.Value.Tags.Contains("TagOne"));
                Assert.That(result.Value.Tags.Contains("TagTwo"));
                Assert.That(result.Value.Tags.Count, Is.EqualTo(2));
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
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                Assert.That(RuntimePlatform.iOS, Is.EqualTo(result.Value.Platform));
            }
            
            [Test]
            public async Task SHOULD_Extract_Templates()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                var template = result.Value.Templates.First();
                Assert.That(template.NotificationName, Is.EqualTo("DummyTemplate"));
                Assert.That(template.DataProperties.Count, Is.EqualTo(1));
                Assert.That(template.DataProperties.Contains("VisibleTemplateProperty"));
            }

        }

        public class LoadAndroidRegistration : LoadRegistrationForUserDeviceAsyncTests
        {
            [Test]
            public async Task IF_Platform_is_FCM_SHOULD_Extract_Templates()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                var template = result.Value.Templates.First();
                Assert.That(template.NotificationName, Is.EqualTo("DummyTemplate"));
                Assert.That(template.DataProperties.Count, Is.EqualTo(2));
                Assert.That(template.DataProperties.Contains("DummyPropertyOne"));
                Assert.That(template.DataProperties.Contains("DummyPropertyTwo"));
            }

            [Test]
            public async Task WHEN_Platform_is_FCM_SHOULD_return_platform_Android()
            {
                //Arrange
                _installation.Platform = NotificationPlatform.Fcm;

                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                Assert.That(RuntimePlatform.Android, Is.EqualTo(result.Value.Platform));
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
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                Assert.That(RuntimePlatform.UWP, Is.EqualTo(result.Value.Platform));
            }
            
            [Test]
            public async Task SHOULD_Extract_Templates()
            {
                //Act
                var result = await Sut.LoadRegistrationForUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

                //Assert
                var template = result.Value.Templates.First();
                Assert.That(template.NotificationName, Is.EqualTo("Visible"));
                Assert.That(template.DataProperties.Count, Is.EqualTo(3));
                Assert.That(template.DataProperties.Contains("message"));
                Assert.That(template.DataProperties.Contains("exclusive"));
                Assert.That(template.DataProperties.Contains("integer"));
                Assert.That(template.DataProperties.Count,Is.EqualTo(3));
            }

        }
    }
}