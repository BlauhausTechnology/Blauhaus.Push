using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Responses;

namespace Blauhaus.Push.Abstractions.Server
{
    public interface ITargetedPushNotificationsServerService
    {
        /// <summary>
        /// Sends notifications directly to PnsHandles, bypassing the Azure templating system.
        /// This requires your Azure notification hub namespace to be at least at the Basic Tier 
        /// </summary>
        Task<Response> SendNotificationToTargetAsync(IPushNotification notification, IDeviceTarget target, IPushNotificationsHub hub);

    }
}