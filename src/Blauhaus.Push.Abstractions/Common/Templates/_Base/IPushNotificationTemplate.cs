using System.Collections.Generic;

namespace Blauhaus.Push.Abstractions.Common.Templates._Base
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