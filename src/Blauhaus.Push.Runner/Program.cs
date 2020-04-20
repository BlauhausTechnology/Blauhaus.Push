using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Common.ValueObjects.BuildConfigs;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Common.Templates;
using Blauhaus.Push.Abstractions.Common.Templates._Base;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Runner.Config;
using Blauhaus.Push.Runner.Config.MineGame;
using Blauhaus.Push.Runner.Config.Reveye;
using Blauhaus.Push.Server._Ioc;
using Blauhaus.Push.Server.Service;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Push.Runner
{
    internal class Program
    {
        private static string NotificationHubPath;
        private static string PnsHandle;
        private static IRuntimePlatform Platform;
        private static string DeviceId;
        private static string ConnectionString;
        private static BasePushRunnerHub Hub;
        private static string UserId;

        private static IPushNotificationsServerService PushNotificationsService;

        private static async Task Main(string[] args)
        {
            //PushNotificationsService = Setup(new AspersUwpHub());
            PushNotificationsService = Setup(new RainbowUwpHub());

            var template = Templates.Message;

            try
            {
                //await PushNotificationsService.UpdateDeviceRegistrationAsync(new DeviceRegistration
                //{
                //    AccountId = "myAccountId",
                //    UserId = "myUserId",
                //    Platform = Platform,
                //    DeviceIdentifier = DeviceId,
                //    PushNotificationServiceHandle = PnsHandle,
                //    Tags = new List<string> { "RandomTaggage" },
                //    Templates = new List<IPushNotificationTemplate>
                //    {
                //        Templates.Message
                //    }
                //}, Hub, CancellationToken.None);


                //var registrations = await GetAllRegistrationsAsync();
                var registrations = await GetRegistrationsForUserAsync("8B3A0775-973F-4442-B227-0CBBD639CA23");
                foreach (var registrationDescription in registrations)
                {
                    var regType = registrationDescription.GetType();
                }

                var reg = await PushNotificationsService.LoadRegistrationForUserDeviceAsync(UserId, DeviceId, Hub, CancellationToken.None);

                await PushNotificationsService.SendNotificationToUserAsync(new MessageNotification(
                    "This is a drill", "Please head to the nearest shed", "Payload data", "Payload id"), "myUserId", Hub, CancellationToken.None);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Todo also when you delete an installation it becomes permanently fucked: you cannot create a new installation for that same deviceId


        }
        
        private static readonly PushNotificationTemplate VisibleTemplate = new PushNotificationTemplate("Visible", "DefaultTitle", "DefaultBody", new List<string>
        {
            "message",
            "exclusive",
            "integer"
        });

        private static IPushNotification VisibleNotification = new PushNotificationBuilder(VisibleTemplate)
            .WithDataProperty("message", "This is the Message")
            .WithDataProperty("exclusive", "Win!")
            .WithDataProperty("integer", "1")
            .Create();



        private static IPushNotificationsServerService Setup(BasePushRunnerHub hub)  
        {
            var services = new ServiceCollection();

            services.AddSingleton<IBuildConfig>(BuildConfig.Debug);

            services.AddPushNotificationsServer(new ConsoleTraceListener());
            NotificationHubPath = hub.NotificationHubName;
            DeviceId = hub.DeviceId;
            PnsHandle = hub.PnsHandle;
            ConnectionString = hub.NotificationHubConnectionString;
            Platform = hub.Platform;
            UserId = hub.UserId;
            
            Hub = hub;

            return services.BuildServiceProvider().GetRequiredService<IPushNotificationsServerService>();
        }

        private static async Task<List<RegistrationDescription>> GetRegistrationsForUserAsync(string userId)
        {
            var client = NotificationHubClient.CreateClientFromConnectionString(ConnectionString, NotificationHubPath);
            var registrations = await client.GetRegistrationsByTagAsync($"UserId_{userId}", 40);
            var count = 0;
            foreach (var reg in registrations)
            {
                count++;
                Console.WriteLine("Reg #: " + count + " Id: " + reg.RegistrationId);
                Console.WriteLine(reg.Serialize());
                Console.WriteLine();
            }

            return registrations.ToList();
        }

        private static async Task<List<RegistrationDescription>> GetAllRegistrationsAsync()
        {
            var client = NotificationHubClient.CreateClientFromConnectionString(ConnectionString, NotificationHubPath);
            var registrations = await client.GetAllRegistrationsAsync(10);
            var count = 0;
            foreach (var reg in registrations)
            {
                count++;
                Console.WriteLine("Reg #: " + count + " Id: " + reg.RegistrationId);
                Console.WriteLine(reg.Serialize());
                Console.WriteLine();
            }

            return registrations.ToList();
        }

    }
}
