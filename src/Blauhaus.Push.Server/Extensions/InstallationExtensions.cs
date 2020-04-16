using System;
using System.Collections.Generic;
using System.Xml;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.PushNotificationTemplates;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json.Linq;

namespace Blauhaus.Push.Server.Extensions
{
    public static class InstallationExtensions
    {
        public static string ExtractUserId(this Installation installation)
        {
            var userId = string.Empty;
            foreach (var tag in installation.Tags)
            {
                if (tag.StartsWith("UserId_"))
                {
                    userId = tag.Substring(7, (tag.Length - 7));
                }
            }
            return userId;
        }

        public static string ExtractAccountId(this Installation installation)
        {
            var accountId = string.Empty;
            foreach (var tag in installation.Tags)
            {
                if (tag.StartsWith("AccountId_"))
                {
                    accountId = tag.Substring(10, (tag.Length - 10));
                }
            }
            return accountId;
        }
       
        public static List<string> ExtractTags(this Installation installation)
        {
            var tags = new List<string>();
            foreach (var tag in installation.Tags)
            {
                if (!tag.StartsWith("AccountId_") && !tag.StartsWith("UserId_"))
                {
                    tags.Add(tag);
                }
            }
            return tags;
        }

        public static List<INotificationTemplate> ExtractTemplates(this Installation installation)
        {
            var templates = new List<INotificationTemplate>();

            foreach (var installationTemplate in installation.Templates)
            {
                if(installation.Platform == NotificationPlatform.Fcm)
                    templates.Add(ExtractAndroidPushNotificationTemplate(installationTemplate));
                
                if(installation.Platform == NotificationPlatform.Apns)
                    templates.Add(ExtractIosPushNotificationTemplate(installationTemplate));
                
                if(installation.Platform == NotificationPlatform.Wns)
                    templates.Add(ExtractUwpPushNotificationTemplate(installationTemplate));
            }

            return templates;
        }
        
        private static INotificationTemplate ExtractIosPushNotificationTemplate(KeyValuePair<string, InstallationTemplate> installationTemplate)
        {
            var templateName = installationTemplate.Key;
            var templateProperties = new List<string>();

            var templateBody = JObject.Parse(installationTemplate.Value.Body);

            foreach (var templateProperty in templateBody)
            {
                if (templateProperty.Key != "aps")
                {
                    templateProperties.Add(templateProperty.Key);
                }
            }
            
            //todo extract default title and body
            return new PushNotificationTemplate(templateName, "", "", templateProperties);
        }

        private static INotificationTemplate ExtractUwpPushNotificationTemplate(KeyValuePair<string, InstallationTemplate> installationTemplate)
        {
            var templateName = installationTemplate.Key;
            var templateProperties = new List<string>();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(installationTemplate.Value.Body);

            var nodeContents = xmlDoc.SelectNodes("/toast/visual/binding");
            if (nodeContents != null && nodeContents.Count > 0)
            {
                var content = nodeContents[0];
                var payload = content.ChildNodes[content.ChildNodes.Count - 1];
                var json = payload.InnerText;

                var templateBody = JObject.Parse(json);

                foreach (var dataProperty in templateBody)
                {
                    templateProperties.Add(dataProperty.Key);
                }

                //todo extract default title and body
                return new PushNotificationTemplate(templateName, "", "", templateProperties);

            }

            throw new Exception("Unable to parse UWP Installation Template");
        }

        private static INotificationTemplate ExtractAndroidPushNotificationTemplate(KeyValuePair<string, InstallationTemplate> installationTemplate)
        {
            var templateName = installationTemplate.Key;
            var templateProperties = new List<string>();

            var templateBody = JObject.Parse(installationTemplate.Value.Body);

            if (templateBody.TryGetValue("data", out var data))
            {
                foreach (var dataProperty in data)
                {
                    if (dataProperty is JProperty jProperty)
                    {
                        templateProperties.Add(jProperty.Name);
                    }
                }
            }
            
            //todo extract default title and body
            return new PushNotificationTemplate(templateName, "", "", templateProperties);
        }
    }
}