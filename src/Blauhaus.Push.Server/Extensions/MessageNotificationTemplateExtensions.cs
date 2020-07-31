using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.Templates._Base;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.Extensions
{
    public static class MessageNotificationTemplateExtensions
    {
        public static KeyValuePair<string, InstallationTemplate> ToPlatform(this IPushNotificationTemplate template, IRuntimePlatform platform)
        {
            if (platform.Value == RuntimePlatform.Android.Value)
                return new KeyValuePair<string, InstallationTemplate>(template.NotificationName, ToAndroid(template));
            
            if (platform.Value == RuntimePlatform.iOS.Value)
                return new KeyValuePair<string, InstallationTemplate>(template.NotificationName, ToIos(template));
            
            if (platform.Value == RuntimePlatform.UWP.Value)
                return new KeyValuePair<string, InstallationTemplate>(template.NotificationName, ToUwp(template));

            throw new ArgumentException($"{platform.Value} is not supported");
        }

        private static InstallationTemplate ToUwp(IPushNotificationTemplate template)
        {
            var launchProperties = new StringBuilder();


            if (template.DataProperties.Count > 0)
            {
                
                launchProperties.Append("{'{' + ");

                launchProperties
                    .Append("'Title'").Append(" + ':' + ").Append("'%22' + ").Append("$(Title)").Append(" + '%22'").Append(" + ', ' + ")
                    .Append("'Body'").Append(" + ':' + ").Append("'%22' + ").Append("$(Body)").Append(" + '%22'").Append(" + ', ' + ")
                    .Append("'Template_Name'").Append(" + ':' + ").Append("'%22' + '").Append(template.NotificationName).Append("' + '%22'").Append(" + ', ' + ");

                for (var i = 0; i < template.DataProperties.Count; i++)
                {
                    if (i != 0) launchProperties.Append(" + ', ' + ");
                    launchProperties
                        .Append("'").Append(template.DataProperties[i]).Append("'")
                        .Append(" + ':' + ")
                        .Append("'%22' + ")
                        .Append("$(").Append(template.DataProperties[i]).Append(")")
                        .Append(" + '%22'");
                }

                launchProperties.Append(" + '}'}");

                Debug.WriteLine(launchProperties.ToString());
            }

            var body = new StringBuilder();
            body.Append($"<toast launch=\"{launchProperties}\"><visual><binding template=\"ToastText01\">")
                .Append("<text id=\"1\">$(Title)</text>")
                .Append("<text id=\"2\">$(Body)</text>")
                .Append("</binding></visual></toast>");

            var b = body.ToString();

            return new InstallationTemplate
            {
                Body = body.ToString(),
                Headers = new Dictionary<string, string>{ {"X-WNS-Type", "wns/toast"} }
            };
        }

        private static InstallationTemplate ToIos(IPushNotificationTemplate template)
        {
            var body = new StringBuilder();

            body.Append("{ ");

            body.Append("\"aps\" : { ");
            body.Append("\"alert\" : { ");
                
            body.Append("\"title\" : \"$(Title)\"").Append(", ");
            body.Append("\"body\" : \"$(Body)\"");
                
            body.Append(" }");
            body.Append(" }, ");
            
            body.Append("\"Template_Name\" : ").Append($"\"{template.NotificationName}\"");

            if (template.DataProperties.Count > 0)
            {
                body.Append(", ");
            }

            for (var i = 0; i < template.DataProperties.Count; i++)
            {
                body.Append("\"").Append(template.DataProperties[i]).Append("\"")
                    .Append(" : ")
                    .Append("\"$(").Append(template.DataProperties[i]).Append(")\"");
                
                if (i < template.DataProperties.Count - 1)
                {
                    body.Append(", ");
                }
            }

            body.Append(" }");

            var b = body.ToString();

            return new InstallationTemplate{ Body = body.ToString()};
        }

        private static InstallationTemplate ToAndroid(IPushNotificationTemplate template)
        {
            var body = new StringBuilder();

            body.Append("{ ");
            body.Append("\"data\" : ");
            body.Append("{ ");

            body.Append("\"Title\" : ").Append("\"$(Title)\"").Append(", ");
            body.Append("\"Body\" : ").Append("\"$(Body)\"").Append(", ");
            body.Append("\"Template_Name\" : ").Append($"\"{template.NotificationName}\"").Append(", ");

            for (var i = 0; i < template.DataProperties.Count; i++)
            {
                body.Append("\"").Append(template.DataProperties[i]).Append("\"")
                    .Append(" : ")
                    .Append("\"$(").Append(template.DataProperties[i]).Append(")\"");

                if (i < template.DataProperties.Count - 1)
                {
                    body.Append(", ");
                }
            }
            body.Append(" }");

            body.Append(", ");
            body.Append("\"notification\" : ");
            body.Append("{ ");
            body.Append("\"title\" : \"$(Title)\"").Append(", ");
            body.Append("\"body\" : \"$(Body)\"");
            body.Append(" }");

            body.Append(" }");

            var b = body.ToString();

            return new InstallationTemplate{ Body = body.ToString()};
        }
    }
}