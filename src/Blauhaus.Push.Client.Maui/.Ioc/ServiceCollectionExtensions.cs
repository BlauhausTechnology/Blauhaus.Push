using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Maui.Common;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Push.Client.Maui.Ioc;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddPushNotificationsClient(this IServiceCollection services, Action<PushNotificationsClientOptions> options)
    {
        return services.AddPushNotificationsClient<EmptyPushNotificationTapHandler>(options);
    }

    public static IServiceCollection AddPushNotificationsClient<TTapHandler>(this IServiceCollection services, Action<PushNotificationsClientOptions> options) where TTapHandler : class, IPushNotificationTapHandler
    {
        services.Configure(options);

        services.TryAddSingleton<IPushNotificationTapHandler, TTapHandler>();

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