using AutoFixture;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.Push.TestHelpers.MockBuilders
{
    public class PushNotificationsHubMockBuilder : BaseMockBuilder<PushNotificationsHubMockBuilder, IPushNotificationsHub>
    {
        public PushNotificationsHubMockBuilder()
        {
            With(x => x.NotificationHubConnectionString, MyFixture.Create<string>());
            With(x => x.NotificationHubName, MyFixture.Create<string>());
        }
    }
}