using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Errors;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Server.Extractors;
using Blauhaus.Responses;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Push.Tests.MockBuilders
{
    public class NativeNotificationExtractorMockBuilder : BaseMockBuilder<NativeNotificationExtractorMockBuilder, INativeNotificationExtractor>
    {
        public NativeNotificationExtractorMockBuilder()
        {
            
        }
        
        public NativeNotificationExtractorMockBuilder Where_ExtractNotification_returns(NativeNotification notification)
        {
            Mock.Setup(x => x.ExtractNotification(It.IsAny<IRuntimePlatform>(), It.IsAny<IPushNotification>()))
                .Returns(Response.Success(notification));
            return this;
        }

        public NativeNotificationExtractorMockBuilder Where_ExtractNotification_fails(Error error)
        {
            Mock.Setup(x => x.ExtractNotification(It.IsAny<IRuntimePlatform>(), It.IsAny<IPushNotification>()))
                .Returns(Response.Failure<NativeNotification>(error));
            return this;
        }

        //public NativeNotificationExtractorMockBuilder Where_ExtractIosNotification_returns(NativeNotification notification, IPushNotification pushNotification = null)
        //{
        //    if (pushNotification == null)
        //    {
        //        Mock.Setup(x => x.ExtractIosNotification(It.IsAny<IPushNotification>()))
        //            .Returns(Response.Ok(notification));
        //    }
        //    else
        //    {
        //        Mock.Setup(x => x.ExtractIosNotification(pushNotification))
        //            .Returns(Response.Ok(notification));
        //    }
        //    return this;
        //}

        //public NativeNotificationExtractorMockBuilder Where_ExtractAndroidNotification_returns(NativeNotification notification, IPushNotification pushNotification = null)
        //{
        //    if (pushNotification == null)
        //    {
        //        Mock.Setup(x => x.ExtractAndroidNotification(It.IsAny<IPushNotification>()))
        //            .Returns(Response.Ok(notification));
        //    }
        //    else
        //    {
        //        Mock.Setup(x => x.ExtractAndroidNotification(pushNotification))
        //            .Returns(Response.Ok(notification));
        //    }
        //    return this;
        //}
        
        //public NativeNotificationExtractorMockBuilder Where_ExtractUwpNotification_returns(NativeNotification notification, IPushNotification pushNotification = null)
        //{
        //    if (pushNotification == null)
        //    {
        //        Mock.Setup(x => x.ExtractUwpNotification(It.IsAny<IPushNotification>()))
        //            .Returns(Response.Ok(notification));
        //    }
        //    else
        //    {
        //        Mock.Setup(x => x.ExtractUwpNotification(pushNotification))
        //            .Returns(Response.Ok(notification));
        //    }
        //    return this;
        //}
    }
}