﻿using System;
using System.Collections.Generic;
using Blauhaus.Push.Abstractions.Common.Notifications;

namespace Blauhaus.Push.Abstractions.Common.Templates._Base
{

    public class PushNotificationBuilder 
    {
        private readonly IPushNotificationTemplate _template;
        private readonly Dictionary<string, object> _dataProperties = new Dictionary<string, object>();
        private string _title;
        private string _body;

        public PushNotificationBuilder(IPushNotificationTemplate template)
        {
            _template = template;
            _title = template.DefaultTitle;
            _body = template.DefaultBody;
        }

        public IPushNotification Create()
        {
            return new PushNotification(_template.NotificationName, _dataProperties, _title, _body);
        }

        public PushNotificationBuilder WithDataProperty(string name, object value)
        {
            if (!_template.DataProperties.Contains(name))
            {
                throw new ArgumentException("The requested data property does not exist in the push notification template");
            }

            _dataProperties[name] = value;

            return this;
        }

        public PushNotificationBuilder WithContent(string title, string body)
        {
            _title = string.IsNullOrWhiteSpace(title) ? _template.DefaultTitle : title;
            _body = string.IsNullOrWhiteSpace(body) ? _template.DefaultBody : body;
            return this;
        }




    }
}