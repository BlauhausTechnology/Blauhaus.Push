using System.Collections.Generic;
using Blauhaus.Push.Abstractions.Server;

namespace Blauhaus.Push.Server.PushNotificationTemplates
{
    public class PushNotificationTemplate : IPushNotificationTemplate
    {

        public PushNotificationTemplate(string notificationName, string defaultTitle, string defaultBody, List<string> properties = null) 
        {
            NotificationName = notificationName;
            DefaultTitle = defaultTitle;
            DefaultBody = defaultBody;
            DataProperties = properties;
        }
        
        public string NotificationName { get; }
        public string DefaultTitle { get; }
        public string DefaultBody { get; }
        public List<string> DataProperties { get; }
    }
}