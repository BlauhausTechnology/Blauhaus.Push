using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.iOS;
using Blauhaus.Push.Client.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Push.Client.Xamarin.iOS.Ioc
{
    public static class IosServiceCollectionExtensions
    {
        public static IServiceCollection AddIosPushNotifications(this IServiceCollection services)
        {
            services.AddSingleton<IosPushNotificationHandler>();
            services.AddSingleton<IPushNotificationsClientService, IosPushNotificationsClientService>();

            return services;
        }

    }
}