using System.Diagnostics;
using Blauhaus.Analytics.Console._Ioc;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Extractors;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.Push.Server.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Push.Server._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPushNotificationsServer(this IServiceCollection services, TraceListener traceListener) 
        {
            services.RegisterConsoleLoggerService(traceListener);
            services.AddTransient<IPushNotificationsServerService, PushNotificationsServerService>();
            services.AddTransient<INotificationHubClientProxy, NotificationHubClientProxy>();
            services.AddTransient<INativeNotificationExtractor, NativeNotificationExtractor>();
            return services;
        }

        public static IServiceCollection AddTargetedPushNotificationsServer(this IServiceCollection services, TraceListener traceListener) 
        {
            services.RegisterConsoleLoggerService(traceListener);
            services.AddTransient<ITargetedPushNotificationsServerService, TargetedPushNotificationsServerService>();
            services.AddTransient<INotificationHubClientProxy, NotificationHubClientProxy>();
            services.AddTransient<INativeNotificationExtractor, NativeNotificationExtractor>();
            return services;
        }
    }
}