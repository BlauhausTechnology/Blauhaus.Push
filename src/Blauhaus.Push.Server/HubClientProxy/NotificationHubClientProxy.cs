using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Common.ValueObjects.BuildConfigs;
using Blauhaus.Push.Server._Config;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.HubClientProxy
{
    public class NotificationHubClientProxy : INotificationHubClientProxy
    {
        private readonly NotificationHubClient _hubClient;

        public NotificationHubClientProxy(IPushNotificationsServerConfig config, IBuildConfig buildConfig)
        {
            var enableTestSend = (BuildConfig) buildConfig == BuildConfig.Debug;

            _hubClient = NotificationHubClient.CreateClientFromConnectionString(
                config.NotificationHubConnectionString, config.NotificationHubName, enableTestSend);
        }


        public Task CreateOrUpdateInstallationAsync(Installation installation, CancellationToken cancellationToken)
        {
            return _hubClient.CreateOrUpdateInstallationAsync(installation, cancellationToken);
        }

        public Task<Installation> GetInstallationAsync(string installationId, CancellationToken token)
        {
            return _hubClient.GetInstallationAsync(installationId, token);
        }

        public Task<bool> InstallationExistsAsync(string installationId, CancellationToken token)
        {
            return _hubClient.InstallationExistsAsync(installationId, token);
        }

        //the NotificationOutcome only has interesting data when you're on the Standard tier :(
        public Task<NotificationOutcome> SendNotificationAsync(IDictionary<string, string> properties, IEnumerable<string> tags, CancellationToken token)
        {
            return _hubClient.SendTemplateNotificationAsync(properties, tags, token);
        }

        public Task<NotificationOutcome> SendNotificationAsync(IDictionary<string, string> properties, string tagExpression, CancellationToken token)
        {
            return _hubClient.SendTemplateNotificationAsync(properties, tagExpression, token);
        }
    }
}