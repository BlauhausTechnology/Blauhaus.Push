using System.Collections.Generic;

namespace Blauhaus.Push.Abstractions.Server
{
    public interface INotificationTemplate
    {
        string DefaultTitle { get; }
        string DefaultBody { get; }
        string Name { get; }
        List<string> DataProperties { get; }
    }
}