using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Server;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.HubClientProxy
{
    public interface INotificationHubClientProxy
    {
        INotificationHubClientProxy Initialize(IPushNotificationsHub hub);

        Task CreateOrUpdateInstallationAsync(Installation installation);
        Task<Installation> GetInstallationAsync(string installationId);
        Task<bool> InstallationExistsAsync(string installationId);
        Task<NotificationOutcome> SendNotificationAsync(IDictionary<string, string> properties, IEnumerable<string> tags);
        Task<NotificationOutcome> SendNotificationAsync(IDictionary<string, string> properties, string tagExpression);
        Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, List<string> pnsHandles);
    }
}