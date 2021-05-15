using Blauhaus.Ioc.Abstractions;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Common.Config;

namespace Blauhaus.Push.Client.Common.Ioc
{
    public static class IocServiceExtensions
    {
        public static IIocService AddPushNotificationsClient<TConfig, TPushNotificationHandler>(this IIocService iocService)
            where TPushNotificationHandler : class, IPushNotificationTapHandler 
            where TConfig : class, IPushNotificationsClientConfig
        {
            
            return iocService.AddClient<TConfig, TPushNotificationHandler>();
        } 

        public static IIocService AddPushNotificationsClient<TConfig>(this IIocService iocService) 
            where TConfig : class, IPushNotificationsClientConfig
        {
            return iocService.AddClient<TConfig, EmptyPushNotificationTapHandler>();
        }

        private static IIocService AddClient<TConfig, THandler>(this IIocService iocService) 
            where TConfig : class, IPushNotificationsClientConfig 
            where THandler : class,  IPushNotificationTapHandler
        {
            
            iocService.RegisterType<THandler>(IocLifetime.Singleton);
            iocService.RegisterImplementation<IPushNotificationTapHandler, THandler>(IocLifetime.Singleton);
            iocService.RegisterImplementation<IPushNotificationsClientConfig, TConfig>(IocLifetime.Singleton);
            return iocService;
        }
    }
}