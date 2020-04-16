using System.Collections.Generic;
using Blauhaus.Push.Abstractions.Common.PushNotificationTemplates;
using Blauhaus.Push.Abstractions.Common.PushNotificationTemplates._Base;

namespace Blauhaus.Push.Server.PushNotificationTemplates._Base
{
    public abstract class BaseNotificationTemplate : INotificationTemplate
    {
        protected BaseNotificationTemplate(string type, List<string> properties)
        {
            NotificationType = type;
            DataProperties = properties ?? new List<string>();
        }

        public string NotificationType { get; }
        public List<string> DataProperties { get; }


        
    }
}