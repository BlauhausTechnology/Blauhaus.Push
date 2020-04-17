using System.Collections.Generic;
using Blauhaus.Push.Abstractions.Common.Templates._Base;

namespace Blauhaus.Push.Abstractions.Common.Templates
{
    public static class Templates
    {
        public static PushNotificationTemplate Message = new PushNotificationTemplate(
            "Message", "New Message", "Message",  new List<string>
            {
                "Payload",
                "Id"
            });
    }
}