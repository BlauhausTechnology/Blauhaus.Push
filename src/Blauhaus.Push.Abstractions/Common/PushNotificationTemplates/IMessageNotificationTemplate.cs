using Blauhaus.Push.Abstractions.Common.PushNotificationTemplates._Base;

namespace Blauhaus.Push.Abstractions.Common.PushNotificationTemplates
{
    public interface IMessageNotificationTemplate : INotificationTemplate
    {
        string DefaultTitle { get; }
        string DefaultBody { get; }

        //todo badge
        //todo sound
    }
}