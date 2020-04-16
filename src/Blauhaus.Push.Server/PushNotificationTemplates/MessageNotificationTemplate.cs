using System.Collections.Generic;
using Blauhaus.Push.Abstractions.Common.PushNotificationTemplates;
using Blauhaus.Push.Server.PushNotificationTemplates._Base;

namespace Blauhaus.Push.Server.PushNotificationTemplates
{
    public class MessageNotificationTemplate : BaseNotificationTemplate, IMessageNotificationTemplate
    {

        public MessageNotificationTemplate(string notificationType, string defaultTitle, string defaultBody, List<string> properties = null) 
            : base(notificationType, properties)
        {
            DefaultTitle = defaultTitle;
            DefaultBody = defaultBody;
        }

        public string DefaultTitle { get; }
        public string DefaultBody { get; }
    }
}