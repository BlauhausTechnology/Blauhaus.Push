using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Common.Templates._Base;

namespace Blauhaus.Push.Abstractions.Common.Templates
{
    public class MessageNotification : PushNotification
    {

        public MessageNotification(string title, string body, string payload = "", string id = "") 
            : base(new PushNotificationBuilder(Templates.Message)
                .WithContent(title, body)
                .WithDataProperty("Id", id)
                .WithDataProperty("Payload", payload))
        {
        }
    }
}