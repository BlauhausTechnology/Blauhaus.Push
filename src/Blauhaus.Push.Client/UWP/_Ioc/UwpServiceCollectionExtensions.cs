using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Common;
using Blauhaus.Push.Client.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Push.Client.UWP._Ioc
{
    public static class UwpServiceCollectionExtensions
    {
        public static IServiceCollection AddUwpPushNotifications(this IServiceCollection services)
        {
            services.AddSingleton<UwpPushNotificationHandler>();
            services.AddSingleton<IPushNotificationsClientService, UwpPushNotificationsClientService>();

            return services;
        }
    }
}