using System.Threading.Tasks;
using Blauhaus.Push.Tests.Client.Tests._Base;
using Blauhaus.Push.Tests.Client.Tests.PushNotificationsClientServiceTests.BasePushNotificationsClientServiceTests._Base;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Client.Tests.PushNotificationsClientServiceTests.BasePushNotificationsClientServiceTests
{
    public class GetPushNotificationServiceHandleAsyncTests : BasePushTest<TestPushNotificationsClientService>
    {
        public override void Setup()
        {
            base.Setup();
            Services.AddSingleton<TestPushNotificationsClientService>();
            MockSecureStorageService.Where_GetAsync_returns("stored handle", "PnsHandle");
        }

        [Test]
        public async Task SHOULD_load_and_return_handle_from_secure_storage()
        {
            //Act
            var result = await Sut.GetPushNotificationServiceHandleAsync();

            //Assert
            Assert.AreEqual("stored handle", result);
            MockAnalyticsService.VerifyTrace("PnsHandle loaded");
            MockAnalyticsService.VerifyTraceProperty("PnsHandle", "stored handle");
        }

        [Test]
        public async Task IF_no_stored_handle_exists_SHOULD_return_empty_string_and_trace()
        {
            //Arrange
            MockSecureStorageService.Where_GetAsync_returns(null, "PnsHandle");

            //Act
            var result = await Sut.GetPushNotificationServiceHandleAsync();

            //Assert
            Assert.AreEqual(string.Empty, result);
            MockAnalyticsService.VerifyTrace("No PnsHandle found");
        }

        [Test]
        public async Task SHOULD_cache_loaded_value()
        {
            //Act
            var result1 = await Sut.GetPushNotificationServiceHandleAsync();
            var result2 = await Sut.GetPushNotificationServiceHandleAsync();

            //Assert
            Assert.AreEqual("stored handle", result1);
            Assert.AreEqual("stored handle", result2);
            MockSecureStorageService.Mock.Verify(x => x.GetAsync("PnsHandle"), Times.Once);
        }
    }
}