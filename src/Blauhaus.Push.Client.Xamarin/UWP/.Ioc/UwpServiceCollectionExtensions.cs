﻿using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Push.Client.Xamarin.UWP.Ioc
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