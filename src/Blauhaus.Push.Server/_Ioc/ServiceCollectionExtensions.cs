﻿using System.Diagnostics;
using Blauhaus.Analytics.Console._Ioc;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server._Config;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.Push.Server.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Push.Server._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPushNotificationsServer<TConfig>(this IServiceCollection services, TraceListener traceListener) 
            where TConfig : class, IPushNotificationsServerConfig
        {
            services.AddTransient<IPushNotificationsServerConfig, TConfig>();
            services.RegisterConsoleLoggerService(traceListener);
            services.AddScoped<IPushNotificationsServerService, AzurePushNotificationsServerService>();
            services.AddScoped<INotificationHubClientProxy, NotificationHubClientProxy>();
            return services;
        }
    }
}