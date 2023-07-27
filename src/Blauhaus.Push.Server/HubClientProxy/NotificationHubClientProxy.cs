using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Common.ValueObjects.BuildConfigs;
using Blauhaus.Push.Abstractions.Server;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.HubClientProxy
{
    public class NotificationHubClientProxy : INotificationHubClientProxy
    {
        private NotificationHubClient? _hubClient;
        private readonly bool _enableTestSend;

        public NotificationHubClientProxy(IBuildConfig buildConfig)
        {
            //todo test send not available for sending to device handles

            _enableTestSend = (BuildConfig) buildConfig == BuildConfig.Debug;

        }

        public INotificationHubClientProxy Initialize(IPushNotificationsHub hub)
        {

            //todo this used to be required but suddenly started returning 401s. Leaving this note here in case it blows up again. 20/Oct/2020
            //if (!hub.NotificationHubConnectionString.Contains(hub.NotificationHubName))
            //{
            //    throw new Exception("Invalid notification hub configuration");
            //}

            _hubClient = NotificationHubClient.CreateClientFromConnectionString(
                hub.NotificationHubConnectionString, hub.NotificationHubName, _enableTestSend);
            return this;
        }

        public Task CreateOrUpdateInstallationAsync(Installation installation)
        {
            return GetClient().CreateOrUpdateInstallationAsync(installation);
        }

        public Task<Installation> GetInstallationAsync(string installationId)
        {
            return GetClient().GetInstallationAsync(installationId);
        }

        public async Task<bool> InstallationExistsAsync(string installationId)
        {
            try
            {
                NotificationHubClient client = GetClient();
                bool result = await client.InstallationExistsAsync(installationId);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        //the NotificationOutcome only has interesting data when you're on the Standard tier :(
        public Task<NotificationOutcome> SendNotificationAsync(IDictionary<string, string> properties, IEnumerable<string> tags)
        {
            return GetClient().SendTemplateNotificationAsync(properties, tags);
        }

        public Task<NotificationOutcome> SendNotificationAsync(IDictionary<string, string> properties, string tagExpression)
        {
            return GetClient().SendTemplateNotificationAsync(properties, tagExpression);
        }

        public Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, List<string> pnsHandles)
        {
            return GetClient().SendDirectNotificationAsync(notification, pnsHandles);
        }


        private NotificationHubClient GetClient()
        {
            if (_hubClient == null)
            {
                throw new Exception("Notification hub client has not been initialized. Please call Initialize() before using the hub");
            }

            return _hubClient;
        }

    }
}