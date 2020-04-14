using System.Collections.Generic;

namespace Blauhaus.Push.Abstractions
{
    public interface IPushNotification
    {
        Dictionary<string, object> DataProperties { get; }

        string Type { get; }
        string Title { get; }
        string Body { get; }
    }
}