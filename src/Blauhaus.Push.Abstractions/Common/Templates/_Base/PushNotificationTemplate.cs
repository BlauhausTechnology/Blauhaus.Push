using System.Collections.Generic;

namespace Blauhaus.Push.Abstractions.Common.Templates._Base
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