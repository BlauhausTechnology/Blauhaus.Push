using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.HubClientProxy
{
    public interface INotificationHubClientProxy
    {
        Task CreateOrUpdateInstallationAsync(Installation installation, CancellationToken cancellationToken);

        Task<Installation> GetInstallationAsync(string installationId, CancellationToken token);
        Task<bool> InstallationExistsAsync(string installationId, CancellationToken token);
        Task<NotificationOutcome> SendNotificationAsync(IDictionary<string, string> properties, IEnumerable<string> tags, CancellationToken token);
        Task<NotificationOutcome> SendNotificationAsync(IDictionary<string, string> properties, string tagExpression, CancellationToken token);
    }
}