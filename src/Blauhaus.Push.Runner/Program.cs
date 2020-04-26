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
using Blauhaus.Push.Server.HubClientProxy;
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
        private static IDeviceTarget Target;

        private static NotificationHubClient RawClient => NotificationHubClient.CreateClientFromConnectionString(ConnectionString, NotificationHubPath);
        private static NotificationHubClientProxy Client;
        private static IPushNotificationsServerService PushNotificationsService;

        private static async Task Main(string[] args)
        {
            try
            {
                PushNotificationsService = Setup(new GameUwpHub());

                await PushNotificationsService.SendNotificationToUserAsync(new MessageNotification(
                    "This is a grill", "Please head to the nearest shed", "Payload data", "Payload id"), UserId, Hub, CancellationToken.None);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static readonly IPushNotification KingIsDeadAlert = new PushNotification("Alert", new Dictionary<string, object>
        {
            {"Id", Guid.Parse("d76594fd-1cae-4f4f-9dfe-545102d20357") },
            {"Details", "He was stabbed in the bath" },
            {"Number of stabs", 12 },
        }, "Hear Ye mortals!", "The king is dead. Long live the king");

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
            Target = hub.DeviceTarget;
            Hub = hub;

            Client = new NotificationHubClientProxy(BuildConfig.Debug);
            Client.Initialize(Hub);

            return services.BuildServiceProvider().GetRequiredService<IPushNotificationsServerService>();
        }


        private async Task UpdateRegistrationAsync()
        {
            await PushNotificationsService.UpdateDeviceRegistrationAsync(new DeviceRegistration
            {
                AccountId = "myAccountId",
                UserId = UserId,
                Platform = Platform,
                DeviceIdentifier = DeviceId,
                PushNotificationServiceHandle = PnsHandle,
                Tags = new List<string> { "RandomTaggage" },
                Templates = new List<IPushNotificationTemplate>
                {
                    Templates.Message
                }
            }, Hub, CancellationToken.None);
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

        private static async Task DeleteAllRegistrationsAsync()
        {
            //TODO NB when deleting a registration you cannot use the same device Id again, so we should actually never do this in production

            var client = NotificationHubClient.CreateClientFromConnectionString(ConnectionString, NotificationHubPath);
            var registrations = await client.GetAllRegistrationsAsync(100);
            var count = 0;
            try
            {
                foreach (var reg in registrations)
                {
                    await client.DeleteRegistrationAsync(reg.RegistrationId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
