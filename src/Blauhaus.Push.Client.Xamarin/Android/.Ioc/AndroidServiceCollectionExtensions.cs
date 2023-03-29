using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Push.Client.Xamarin.Android.Ioc
{
    public static class AndroidServiceCollectionExtensions
    {
        public static IServiceCollection AddAndroidPushNotifications(this IServiceCollection services)
        {
            services.AddScoped<AndroidPushNotificationHandler>();
            services.AddSingleton<IPushNotificationsClientService, AndroidPushNotificationsClientService>();
            return services;
        }
    }
}