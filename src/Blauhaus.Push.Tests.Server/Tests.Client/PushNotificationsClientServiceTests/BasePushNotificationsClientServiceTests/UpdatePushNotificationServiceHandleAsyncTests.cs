using System;
using System.Threading.Tasks;
using Blauhaus.Push.Tests.Tests.Client._Base;
using Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.BasePushNotificationsClientServiceTests._Base;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.BasePushNotificationsClientServiceTests
{
    public class UpdatePushNotificationServiceHandleAsyncTests : BasePushTest<TestPushNotificationsClientService>
    {
        public override void Setup()
        {
            base.Setup();
            Services.AddSingleton<TestPushNotificationsClientService>();
            MockSecureStorageService.Where_GetAsync_returns("PnsHandle", string.Empty);
        }

        [Test]
        public async Task SHOULD_save_new_handle()
        {
            //Act
            await Sut.UpdatePushNotificationServiceHandleAsync("new Handle");

            //Assert
            MockSecureStorageService.VerifySetAsyncCalled("PnsHandle", "new Handle");
        }
         
        [Test]
        public async Task IF_value_is_same_as_previous_one_SHOULD_not_save()
        {
            //Act
            await Sut.UpdatePushNotificationServiceHandleAsync("new Handle");
            await Sut.UpdatePushNotificationServiceHandleAsync("new Handle");

            //Assert
            MockSecureStorageService.Mock.Verify(x => x.SetAsync("PnsHandle", "new Handle"), Times.Once);
        }
        
        [Test]
        public async Task IF_value_is_same_as_stored_one_SHOULD_not_save()
        {
            //Arrange
            MockSecureStorageService.Where_GetAsync_returns("stored Handle");

            //Act
            await Sut.UpdatePushNotificationServiceHandleAsync("stored Handle");

            //Assert
            MockSecureStorageService.Mock.Verify(x => x.SetAsync("PnsHandle", "stored Handle"), Times.Never);
        }

        [Test]
        public async Task IF_value_is_different_than_previous_one_SHOULD_save_new_value()
        {
            //Act
            await Sut.UpdatePushNotificationServiceHandleAsync("new Handle");
            await Sut.UpdatePushNotificationServiceHandleAsync("even newer Handle");

            //Assert
            MockSecureStorageService.Mock.Verify(x => x.SetAsync("PnsHandle", "new Handle"), Times.Once);
            MockSecureStorageService.Mock.Verify(x => x.SetAsync("PnsHandle", "even newer Handle"), Times.Once);
        } 
        [Test]
        public async Task IF_value_is_different_than_previous_one_SHOULD_update_cached_value()
        {
            //Act
            await Sut.UpdatePushNotificationServiceHandleAsync("new Handle");
            await Sut.UpdatePushNotificationServiceHandleAsync("even newer Handle");
            MockSecureStorageService.Mock.Invocations.Clear();

            //Assert
            var result = await Sut.GetPushNotificationServiceHandleAsync();
            MockSecureStorageService.Mock.Verify(x => x.GetAsync("PnsHandle"), Times.Never);
            Assert.AreEqual("even newer Handle", result);
        }

        [Test]
        public async Task SHOULD_cache_value_for_Get()
        {
            //Act
            await Sut.UpdatePushNotificationServiceHandleAsync("new Handle");
            MockSecureStorageService.Mock.Invocations.Clear();

            //Assert
            await Sut.GetPushNotificationServiceHandleAsync();
            MockSecureStorageService.Mock.Verify(x => x.GetAsync("PnsHandle"), Times.Never);
        }

        [TestCase("")]
        [TestCase(null)]
        public void IF_handle_is_null_SHOULD_throw_exception(string invalidHandle)
        {
            //Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await Sut.UpdatePushNotificationServiceHandleAsync(invalidHandle), 
                "Push notification service handle cannot be empty");
            MockSecureStorageService.Mock.Verify(x => x.SetAsync("PnsHandle", It.IsAny<string>()), Times.Never);
        }
    }
}