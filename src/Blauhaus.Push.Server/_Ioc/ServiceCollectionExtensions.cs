using System.Diagnostics;
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
        public static IServiceCollection AddPushNotificationsServer(this IServiceCollection services, TraceListener traceListener) 
        {
            services.RegisterConsoleLoggerService(traceListener);
            services.AddTransient<IPushNotificationsServerService, PushNotificationsServerService>();
            services.AddTransient<INotificationHubClientProxy, NotificationHubClientProxy>();
            return services;
        }
    }
}