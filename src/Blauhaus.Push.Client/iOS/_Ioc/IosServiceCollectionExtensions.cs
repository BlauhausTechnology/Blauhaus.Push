using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Common;
using Blauhaus.Push.Client.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Push.Client.iOS._Ioc
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