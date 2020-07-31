using System;
using System.Text;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.Notifications;
using CSharpFunctionalExtensions;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.Extractors
{
    public class NativeNotificationExtractor : INativeNotificationExtractor
    {
        
        public Result<NativeNotification> ExtractNotification(IRuntimePlatform platform, IPushNotification pushNotification)
        {
            if ((RuntimePlatform) platform == RuntimePlatform.iOS)
                return ExtractIosNotification(pushNotification);
            if ((RuntimePlatform) platform == RuntimePlatform.UWP)
                return ExtractUwpNotification(pushNotification);
            if ((RuntimePlatform) platform == RuntimePlatform.Android)
                return ExtractAndroidNotification(pushNotification);

            throw new NotImplementedException("Only iOS, Android and UWP are supported platforms for push notifications");
        }


        public Result<NativeNotification> ExtractUwpNotification(IPushNotification pushNotification)
        {
            var launchProperties = new StringBuilder();

            if (pushNotification.DataProperties.Count > 0)
            {
                launchProperties.Append("{ ");

                launchProperties
                    .Append("'Title' : '").Append(pushNotification.Title).Append("', ")
                    .Append("'Body' : '").Append(pushNotification.Body).Append("', ")
                    .Append("'Template_Name' : '").Append(pushNotification.Name).Append("'");

                foreach (var dataProperty in pushNotification.DataProperties)
                {
                    launchProperties
                        .Append(", '")
                        .Append(dataProperty.Key).Append("' : '").Append(dataProperty.Value).Append("'");
                }

                launchProperties.Append(" }");
            }

            var body = new StringBuilder();
            body.Append($"<toast launch=\"{launchProperties}\"><visual><binding template=\"ToastText01\">")
                .Append("<text id=\"1\">").Append(pushNotification.Title).Append("</text>")
                .Append("<text id=\"2\">").Append(pushNotification.Body).Append("</text>")
                .Append("</binding></visual></toast>");

            var nativeNotification = new NativeNotification(new WindowsNotification(body.ToString()));
            return Result.Success(nativeNotification);
        }

        public Result<NativeNotification> ExtractIosNotification(IPushNotification pushNotification)
        {
            var body = new StringBuilder();

            body.Append("{ ");

            body.Append("\"aps\" : { \"alert\" : { ");
            body.Append("\"title\" : \"").Append(pushNotification.Title).Append("\", ");
            body.Append("\"body\" : \"").Append(pushNotification.Body).Append("\"");
            body.Append(" }}, ");
            
            body.Append("\"Template_Name\" : ").Append($"\"{pushNotification.Name}\"");

            foreach (var dataProperty in pushNotification.DataProperties)
            {
                body
                    .Append(", \"")
                    .Append(dataProperty.Key).Append("\" : \"").Append(dataProperty.Value).Append("\"");
            }

            body.Append(" }");

            var b = body.ToString();
            
            var nativeNotification = new NativeNotification(new AppleNotification(body.ToString()));
            return Result.Success(nativeNotification);
        }

        public Result<NativeNotification> ExtractAndroidNotification(IPushNotification pushNotification)
        {
            var body = new StringBuilder();

            body.Append("{ ");

            body.Append("\"notification\" : { ");
            body.Append("\"title\" : \"").Append(pushNotification.Title).Append("\", ");
            body.Append("\"body\" : \"").Append(pushNotification.Body).Append("\"");
            body.Append(" }, ");
            
            body.Append("\"data\" : { ");
            body.Append("\"Template_Name\" : ").Append($"\"{pushNotification.Name}\", ");
            body.Append("\"Title\" : ").Append($"\"{pushNotification.Title}\", ");
            body.Append("\"Body\" : ").Append($"\"{pushNotification.Body}\"");

            foreach (var dataProperty in pushNotification.DataProperties)
            {
                body
                    .Append(", \"")
                    .Append(dataProperty.Key).Append("\" : \"").Append(dataProperty.Value).Append("\"");
            }

            body.Append(" } }");

            var nativeNotification = new NativeNotification(new FcmNotification(body.ToString()));
            return Result.Success(nativeNotification);
        }
    }
}