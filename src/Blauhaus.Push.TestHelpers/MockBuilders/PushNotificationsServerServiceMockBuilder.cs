using System;
using System.Linq.Expressions;
using System.Threading;
using Blauhaus.Errors;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Responses;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Push.TestHelpers.MockBuilders
{
    public class PushNotificationsServerServiceMockBuilder : BaseMockBuilder<PushNotificationsServerServiceMockBuilder, IPushNotificationsServerService>
    {
        public PushNotificationsServerServiceMockBuilder Where_UpdateDeviceRegistrationAsync_returns(IDeviceRegistration result)
        {
            Mock.Setup(x => x.UpdateDeviceRegistrationAsync(Any<IDeviceRegistration>(), Any<IPushNotificationsHub>()))
                .ReturnsAsync(Response.Success(result));
            return this;
        }
        
        public PushNotificationsServerServiceMockBuilder Where_UpdateDeviceRegistrationAsync_fails(Error error)
        {
            Mock.Setup(x => x.UpdateDeviceRegistrationAsync(Any<IDeviceRegistration>(), Any<IPushNotificationsHub>()))
                .ReturnsAsync(Response.Failure<IDeviceRegistration>(error));
            return this;
        }

        public PushNotificationsServerServiceMockBuilder Where_LoadRegistrationForUserDeviceAsync_returns(IDeviceRegistration result)
        {
            Mock.Setup(x => x.LoadRegistrationForUserDeviceAsync(AnyString, AnyString, Any<IPushNotificationsHub>()))
                .ReturnsAsync(Response.Success(result));
            return this;
        }
        
        public PushNotificationsServerServiceMockBuilder Where_LoadRegistrationForUserDeviceAsync_fails(Error error)
        {
            Mock.Setup(x => x.LoadRegistrationForUserDeviceAsync(AnyString, AnyString, Any<IPushNotificationsHub>()))
                .ReturnsAsync(Response.Failure<IDeviceRegistration>(error));
            return this;
        }
        
        public PushNotificationsServerServiceMockBuilder Where_DeregisterUserDeviceAsync_returns(Response result)
        {
            Mock.Setup(x => x.DeregisterUserDeviceAsync(AnyString, AnyString, Any<IPushNotificationsHub>()))
                .ReturnsAsync(result);
            return this;
        }

         

    }
}