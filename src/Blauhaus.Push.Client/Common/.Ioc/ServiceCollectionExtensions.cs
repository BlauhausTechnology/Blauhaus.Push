using Blauhaus.DeviceServices.Common.Ioc;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Common.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Push.Client.Common.Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPushNotificationsClient<TConfig, TPushNotificationHandler>(this IServiceCollection services)
            where TPushNotificationHandler : class, IPushNotificationTapHandler 
            where TConfig : class, IPushNotificationsClientConfig
        {
            
            return services.AddClient<TConfig, TPushNotificationHandler>();
        } 

        public static IServiceCollection AddPushNotificationsClient<TConfig>(this IServiceCollection services) 
            where TConfig : class, IPushNotificationsClientConfig
        {
            return services.AddClient<TConfig, EmptyPushNotificationTapHandler>();
        }

        private static IServiceCollection AddClient<TConfig, THandler>(this IServiceCollection services) 
            where TConfig : class, IPushNotificationsClientConfig 
            where THandler : class,  IPushNotificationTapHandler
        {
            
            services.AddDeviceServices();
            services.TryAddSingleton<THandler>();
            services.AddSingleton<IPushNotificationTapHandler, THandler>();
            services.AddTransient<IPushNotificationsClientConfig, TConfig>();
            return services;
        }
    }
}