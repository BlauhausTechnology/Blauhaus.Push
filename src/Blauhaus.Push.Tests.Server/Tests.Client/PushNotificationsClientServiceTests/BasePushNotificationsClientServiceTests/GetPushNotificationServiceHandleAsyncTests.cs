﻿using System.Threading.Tasks;
using Blauhaus.Push.Tests.Tests.Client._Base;
using Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.BasePushNotificationsClientServiceTests._Base;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.BasePushNotificationsClientServiceTests
{
    public class GetPushNotificationServiceHandleAsyncTests : BasePushTest<TestPushNotificationsClientService>
    {
        public override void Setup()
        {
            base.Setup();
            Services.AddSingleton<TestPushNotificationsClientService>();
            MockSecureStorageService.Where_GetAsync_returns("PnsHandle", "stored handle");
        }

        [Test]
        public async Task SHOULD_load_and_return_handle_from_secure_storage()
        {
            //Act
            var result = await Sut.GetPushNotificationServiceHandleAsync();

            //Assert
            Assert.That(result, Is.EqualTo("stored handle")); 
        }

        [Test]
        public async Task IF_no_stored_handle_exists_SHOULD_return_empty_string_and_trace()
        {
            //Arrange
            MockSecureStorageService.Where_GetAsync_returns("PnsHandle", null);

            //Act
            var result = await Sut.GetPushNotificationServiceHandleAsync();

            //Assert
            Assert.That(result, Is.EqualTo(string.Empty)); 
        }

        [Test]
        public async Task SHOULD_cache_loaded_value()
        {
            //Act
            var result1 = await Sut.GetPushNotificationServiceHandleAsync();
            var result2 = await Sut.GetPushNotificationServiceHandleAsync();

            //Assert
            Assert.That(result1, Is.EqualTo("stored handle"));
            Assert.That(result2, Is.EqualTo("stored handle"));
            MockSecureStorageService.Mock.Verify(x => x.GetAsync("PnsHandle"), Times.Once);
        }
    }
}