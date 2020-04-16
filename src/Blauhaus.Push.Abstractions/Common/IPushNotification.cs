﻿using System.Collections.Generic;

namespace Blauhaus.Push.Abstractions.Common
{
    public interface IPushNotification
    {
        Dictionary<string, object> DataProperties { get; }

        string Name { get; }
        string Title { get; }
        string Body { get; }
    }
}