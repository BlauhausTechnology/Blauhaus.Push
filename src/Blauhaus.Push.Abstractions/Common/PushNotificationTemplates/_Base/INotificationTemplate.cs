using System.Collections.Generic;

namespace Blauhaus.Push.Abstractions.Common.PushNotificationTemplates._Base
{
    public interface INotificationTemplate
    {

        string NotificationType { get; }
        List<string> DataProperties { get; }
    }
}