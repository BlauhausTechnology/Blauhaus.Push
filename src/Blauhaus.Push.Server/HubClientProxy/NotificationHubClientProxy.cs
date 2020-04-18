﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Common.ValueObjects.BuildConfigs;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server._Config;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.HubClientProxy
{
    public class NotificationHubClientProxy : INotificationHubClientProxy
    {
        private NotificationHubClient _hubClient;
        private readonly bool _enableTestSend;

        public NotificationHubClientProxy(IBuildConfig buildConfig)
        {
             _enableTestSend = (BuildConfig) buildConfig == BuildConfig.Debug;
        }

        public INotificationHubClientProxy Initialize(IPushNotificationsHub hub)
        {
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(
                hub.NotificationHubConnectionString, hub.NotificationHubName, _enableTestSend);
            return this;
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