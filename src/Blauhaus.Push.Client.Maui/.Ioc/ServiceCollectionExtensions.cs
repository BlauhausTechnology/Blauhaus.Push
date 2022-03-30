using Blauhaus.Push.Abstractions.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Push.Client.Maui.Ioc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPushNotificationsClient(this IServiceCollection services, Action<PushNotificationsClientOptions> options)
    {
        services.Configure(options);

#if IOS
        services.TryAddSingleton<IosPushNotificationHandler>();
        services.TryAddSingleton<IPushNotificationsClientService, IosPushNotificationsClientService>();

#elif ANDROID
        services.TryAddSingleton<AndroidPushNotificationHandler>();
        services.TryAddSingleton<IPushNotificationsClientService, AndroidPushNotificationsClientService>();
        
#elif WINDOWS10_0_19041_0
        services.TryAddSingleton<WindowsPushNotificationHandler>();
        services.TryAddSingleton<IPushNotificationsClientService, WindowsPushNotificationsClientService>();
 
         
#endif

        return services;
    }
}