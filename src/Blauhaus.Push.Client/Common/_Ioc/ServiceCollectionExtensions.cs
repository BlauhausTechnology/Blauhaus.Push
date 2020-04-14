using System;
using Blauhaus.DeviceServices.Common._Ioc;
using Blauhaus.Push.Abstractions.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Push.Client.Common._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPushNotificationsClient(this IServiceCollection services, Action<PushNotificationsClientConfig> options)
        {
            return services.AddPushNotificationsClient<EmptyPushNotificationTapHandler>(options);
        } 

        public static IServiceCollection AddPushNotificationsClient<TPushNotificationHandler>(this IServiceCollection services, Action<PushNotificationsClientConfig> options) where TPushNotificationHandler : class, IPushNotificationTapHandler
        {
            services.Configure(options);
            services.AddDeviceServices();

            services.TryAddSingleton<TPushNotificationHandler>();
            services.AddSingleton<IPushNotificationTapHandler, TPushNotificationHandler>();
            
            return services;
        } 
        public static IServiceCollection AddPushNotificationTapHandler<TPushNotificationHandler>(this IServiceCollection services) where TPushNotificationHandler : class, IPushNotificationTapHandler
        {
            services.AddSingleton<IPushNotificationTapHandler, TPushNotificationHandler>();
            return services;
        } 

        public static IServiceCollection AddPushNotificationsClient(this IServiceCollection services, PushNotificationsClientConfig config)
        {
            Action<PushNotificationsClientConfig> options = clientConfig =>
            {
                clientConfig.ConnectionString = config.ConnectionString;
                clientConfig.NotificationHubName = config.NotificationHubName;
            };

            services.Configure(options);

            return services;
        } 
    }
}