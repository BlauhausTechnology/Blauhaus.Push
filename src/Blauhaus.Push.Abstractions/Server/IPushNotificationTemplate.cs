using System.Collections.Generic;

namespace Blauhaus.Push.Abstractions.Server
{
    public interface IPushNotificationTemplate
    {

        string NotificationName { get; }
        string DefaultTitle { get; }
        string DefaultBody { get; }
        List<string> DataProperties { get; }

        //todo badge
        //todo sound
    }
}