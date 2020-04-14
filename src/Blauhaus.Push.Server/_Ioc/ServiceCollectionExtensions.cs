using System;
using System.Diagnostics;
using Blauhaus.Analytics.Console._Ioc;
using Blauhaus.Push.Abstractions;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.Push.Server.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Push.Server._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPushNotificationsServer(this IServiceCollection services, Action<PushNotificationsServerConfig> options, TraceListener traceListener) 
        {
            services.Configure(options);

            services.RegisterConsoleLoggerService(traceListener);
            services.AddScoped<IPushNotificationsServerService, AzurePushNotificationsServerService>();
            services.AddScoped<INotificationHubClientProxy, NotificationHubClientProxy>();
            return services;
        }
    }
}