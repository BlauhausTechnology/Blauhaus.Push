using System.Collections.Generic;
using System.Text;

namespace Blauhaus.Push.Abstractions.Common
{
    public class PushNotification : IPushNotification
    {
        public PushNotification(string type, Dictionary<string, object> dataProperties, string title = "", string body = "")
        {
            DataProperties = dataProperties;
            NotificationType = type;
            Title = title;
            Body = body;
        }
        
        public Dictionary<string, object> DataProperties { get; }
        public string NotificationType { get; }
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