using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Console.Ioc;
using Blauhaus.Common.ValueObjects.BuildConfigs;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Common.Templates;
using Blauhaus.Push.Abstractions.Common.Templates._Base;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Runner.Config;
using Blauhaus.Push.Runner.Config.MineGame;
using Blauhaus.Push.Runner.Config.Reveye;
using Blauhaus.Push.Server.Extensions;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.Push.Server.Ioc;
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
        private static IPushNotificationsServerService _pushNotificationsService;

        private static async Task Main(string[] args)
        {
            try
            {
                var hub = new AspersAndroidHub();
                
                _pushNotificationsService = Setup(hub);
                var registrationForUserDevice = await _pushNotificationsService.LoadRegistrationForUserDeviceAsync("f9fcbe14-b94f-4bce-b969-4eb0beb870c0", "52dab57d459926dc", hub);

                if (registrationForUserDevice.IsSuccess && registrationForUserDevice.Value.Templates.Any())
                {
                    await _pushNotificationsService.SendNotificationToUserDeviceAsync(
                        notification: new MessageNotification(title: "Hi Charles", body: "Let me know if you get this", payload: "Payload data", id: "Payload id"), 
                        userId: registrationForUserDevice.Value.UserId,
                        deviceIdentifier: registrationForUserDevice.Value.DeviceIdentifier,
                        hub: Hub);
                }


               

            }
            catch (Exception e)
            {
                Console.WriteLine(value: e);
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
            services.AddSingleton(typeof(IAnalyticsLogger<>), typeof(DummyLogger<>));
            

            services.AddPushNotificationsServer();
            services.RegisterConsoleLoggerService(new ConsoleTraceListener());
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
            await _pushNotificationsService.UpdateDeviceRegistrationAsync(new DeviceRegistration
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
            }, Hub);
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

        private static async Task<List<RegistrationDescription>> GetAllRegistrationsAsync(int limit = 10)
        {
            var client = NotificationHubClient.CreateClientFromConnectionString(ConnectionString, NotificationHubPath);
            var registrations = await client.GetAllRegistrationsAsync(limit);
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

        private static async Task<Installation> GetInstallationAsync(RegistrationDescription registrationDescription)
        {
            
            var device = registrationDescription.Tags.First(x => x.StartsWith("Device"));
            var deviceId = device.Replace("DeviceIdentifier_", "");
            
            var user = registrationDescription.Tags.First(x => x.StartsWith("User"));
            var userId = user.Replace("UserId_", "");
            var installationId = $"{userId}___{deviceId}";

            var client = NotificationHubClient.CreateClientFromConnectionString(ConnectionString, NotificationHubPath);
            var installation = await client.GetInstallationAsync(installationId);
            return installation;
        }
        private static async Task ClearAsync(Installation installation)
        {
            var client = NotificationHubClient.CreateClientFromConnectionString(ConnectionString, NotificationHubPath);
            installation.Templates = new Dictionary<string, InstallationTemplate>();
            await client.CreateOrUpdateInstallationAsync(installation);
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
