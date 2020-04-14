using System.Collections.Generic;
using Blauhaus.Push.Abstractions;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Templates._Base;

namespace Blauhaus.Push.Server.Templates
{
    public class NotificationTemplate : BaseNotificationTemplate, INotificationTemplate
    {

        public NotificationTemplate(string name, string defaultTitle, string defaultBody, List<string> properties = null) 
            : base(name, properties)
        {
            DefaultTitle = defaultTitle;
            DefaultBody = defaultBody;
        }

        public string DefaultTitle { get; }
        public string DefaultBody { get; }
    }
}