using System;
using System.Collections.Generic;
using System.Xml;
using Blauhaus.Common.Utils.Extensions;
using Blauhaus.Push.Abstractions.Common.Templates;
using Blauhaus.Push.Abstractions.Common.Templates._Base;
using Blauhaus.Push.Abstractions.Server;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
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

        public static List<IPushNotificationTemplate> ExtractTemplates(this Installation installation)
        {
            var templates = new List<IPushNotificationTemplate>();

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
        
        private static IPushNotificationTemplate ExtractIosPushNotificationTemplate(KeyValuePair<string, InstallationTemplate> installationTemplate)
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

        private static IPushNotificationTemplate ExtractUwpPushNotificationTemplate(KeyValuePair<string, InstallationTemplate> installationTemplate)
        {
            var templateName = installationTemplate.Key;
            var templateProperties = new List<string>();
            var title = "";
            var body = "";

            var uwpPayload = installationTemplate.Value.Body.ExtractValueBetweenText("<toast launch=\"", "\">");
            var split = uwpPayload.Split('(', ')');
            foreach (var sub in split)
            {
                if (!sub.Contains("'%22'"))
                {
                    if (sub.Equals("Title", StringComparison.InvariantCultureIgnoreCase))
                    {
                        title = sub;
                    }
                    else if (sub.Equals("Body", StringComparison.InvariantCultureIgnoreCase))
                    {
                        body = sub;
                    }
                    else
                    {
                        templateProperties.Add(sub);
                    }
                }
            }
            return new PushNotificationTemplate(templateName, title, body, templateProperties);
        }

        private static IPushNotificationTemplate ExtractAndroidPushNotificationTemplate(KeyValuePair<string, InstallationTemplate> installationTemplate)
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