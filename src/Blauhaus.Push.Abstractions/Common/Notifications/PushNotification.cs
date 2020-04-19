using System.Collections.Generic;
using System.Text;
using Blauhaus.Push.Abstractions.Common.Templates._Base;

namespace Blauhaus.Push.Abstractions.Common.Notifications
{
    public class PushNotification : IPushNotification
    {
        public PushNotification(string type, Dictionary<string, object> dataProperties, string title = "", string body = "")
        {
            DataProperties = dataProperties;
            Name = type;
            Title = title;
            Body = body;
        }

        public PushNotification(PushNotificationBuilder builder)
        {
            var notification = builder.Create();
            DataProperties = notification.DataProperties;
            Name = notification.Name;
            Title = notification.Title;
            Body = notification.Body;
        }
        
        public Dictionary<string, object> DataProperties { get; }
        public string Name { get; }
        public string Title { get; }
        public string Body { get; }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append(Title).Append(" :: ").Append(Body).Append(" \n");
            foreach (var dataProperty in DataProperties)
            {
                s.Append(dataProperty.Key).Append(": ").Append(dataProperty.Value).Append(" | ");
            }

            return s.ToString();
        }
    }
}