using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Common.ValueObjects.BuildConfigs;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.Templates;
using Blauhaus.Push.Abstractions.Common.Templates._Base;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Runner.Config;
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

        private static IPushNotificationsServerService PushNotificationsService;


        #region dev admin ios sandbox

        //private const string ConnectionString =
        //    "Endpoint=sb://minegamedevadminiossandbox.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=kDIla2IiV9ESJWsmoyWbse4i9dGq2HlVA2dIaNH1m/k=;" +
        //    "EntityPath=minegamedevadminiossandbox";
        //private const string NotificationHubPath = "minegamedevadminiossandbox";
        //private const string PnsHandle = "458060045B18CDBD4C20ACA5561D9C7DCBF726D69A135A1CCB86DDEA087B6B53";
        //private static readonly IRuntimePlatform Platform = RuntimePlatform.iOS;
        //private const string DeviceId = "myIosDeviceId";

        #endregion

        #region dev admin 

        //private const string ConnectionString =
        //    "Endpoint=sb://minegamedevadmin.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=6ULhJh6mDzlRr6r0dZ2T6dL8MZHHve10nMq2V36c2T0=;" +
        //    "EntityPath=minegamedevadmin";
        //private const string NotificationHubPath = "minegamedevadmin";

        //////Android
        //private static readonly IRuntimePlatform Platform = RuntimePlatform.Android;
        //private const string DeviceId = "myAndroidDeviceId";
        //private const string PnsHandle = "d51fpqa0o-k:APA91bFwzZ5OuJzLl3qY8Ty39AkEt--tV4q1RGPineWKMGaqK954V0EKOT951iZiMtebnxtC2CZrlPj5_7vaU3UKFkcERcsGlSWuXX7FJyyMY-kCotzqz5EJrou8abo3pO2W51l31yuo";

        //UWP
        //private const string PnsHandle = "https://db5p.notify.windows.com/?token=AwYAAAB6THR%2b7ABipdeJ33UM7Ljd9ctnj8MQkfgr29zWrZVq7BsnVzMpGKDi7hcBVF16h%2bIiq%2fVvBBbNO8JRiSDEUfoiHC1Q1IQ9OY%2fghcad84Sy0ocQrZYRODk2jaymcOGm04wLnGeVWQcyqMc6Q3YHkwDJ";
        //private static readonly IRuntimePlatform Platform = RuntimePlatform.UWP;
        //private const string DeviceId = "myUwpDeviceId";

        #endregion

        #region dev game

        //private const string ConnectionString =
        //    "Endpoint=sb://minegamedevhub.servicebus.windows.net/;" +
        //    "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
        //    "SharedAccessKey=fa802OEeFAWRmXRWTsBzXTRcHk4i3UOiwIgfj68O2Zk=;" +
        //    "EntityPath=minegamedevhub";
        //private const string NotificationHubPath = "minegamedevhub";

        ////UWP
        //private const string PnsHandle = "https://am3p.notify.windows.com/?token=AwYAAABAz1jFcb6Csv4nmNuxMyHlD%2fWL5nxTfKEt4Y3z7y2bhNiCTHkiSlDCd3U%2fPS5cQVKlu22CYL8x6RUcQd3yEu561iZlYkd4txi25TUqXMAOh3ngoboGDsdFNeaqt8YcSS1YS5IZfUORqV%2bII7qpR2q%2b";
        //private static readonly IRuntimePlatform Platform = RuntimePlatform.UWP;
        //private const string DeviceId = "myUwpDeviceId";

        ////Android
        //private const string PnsHandle = "ezMgNtoZoUo:APA91bHEuSKwRvkQI8hN1GMo0Zq8tQWmYkUZ998A4p2fmZTgPUNjRSogLXJAVgeVnveZBf2LLfc4RTE-hyKgPytOfY9VdRMBym-jnxL3194f8d8qENDGFrHZTAtD_Dc_IuQ7oNrXaI3Y";
        //private static readonly IRuntimePlatform Platform = RuntimePlatform.Android;
        //private const string DeviceId = "myAndroidDeviceId";

        #endregion


        private static IPushNotificationsServerService Setup<TConfig>() where TConfig : BasePushRunnerHub, new()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IBuildConfig>(BuildConfig.Debug);

            var setup = new TConfig();
            services.AddPushNotificationsServer(new ConsoleTraceListener());
            NotificationHubPath = setup.NotificationHubName;
            DeviceId = setup.DeviceId;
            PnsHandle = setup.PnsHandle;
            ConnectionString = setup.NotificationHubConnectionString;
            Platform = setup.Platform;

            return services.BuildServiceProvider().GetRequiredService<IPushNotificationsServerService>();
        }


        private static async Task Main(string[] args)
        {

            PushNotificationsService = Setup<AspersAndroidHub>();
            var hub = new AspersAndroidHub();

            try
            {
                var visibleTemplate = new PushNotificationTemplate("Visible", "DefaultTitle", "DefaultBody", new List<string>
                {
                    "message",
                    "exclusive",
                    "integer"
                });

                await PushNotificationsService.UpdateDeviceRegistrationAsync(new DeviceRegistration
                {
                    AccountId = "myAccountId",
                    UserId = "myUserId",
                    Platform = Platform,
                    DeviceIdentifier = DeviceId,
                    PushNotificationServiceHandle = PnsHandle,
                    Tags = new List<string> { "RandomTaggage" },
                    Templates = new List<IPushNotificationTemplate>
                    {
                        visibleTemplate
                    }
                }, hub, CancellationToken.None);


                var reg = PushNotificationsService.LoadDeviceRegistrationAsync(DeviceId, hub, CancellationToken.None);
                

                await PushNotificationsService.SendNotificationToUserAsync(new PushNotificationBuilder(visibleTemplate)
                    .WithDataProperty("message", "This is the Message")
                    .WithDataProperty("exclusive", "Win!")
                    .WithDataProperty("integer", "1")
                    .Create(), "myUserId", hub, CancellationToken.None);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Todo also when you delete an installation it becomes permanently fucked: you cannot create a new installation for that same deviceId


        }

        private static async Task<List<RegistrationDescription>> GetAllRegistrationsAsync(string pnsHandle)
        {
            var client = NotificationHubClient.CreateClientFromConnectionString(ConnectionString, NotificationHubPath);
            var registrations = await client.GetAllRegistrationsAsync(10);
            var count = 0;
            foreach (var reg in registrations)
            {
                count++;
                Console.WriteLine("Reg #: " + count + " Id: " + reg.RegistrationId);
                Console.WriteLine();
                if (reg.PnsHandle == pnsHandle)
                {
                    Console.WriteLine(reg.Serialize());
                }
            }

            return registrations.ToList();
        }

        //private static async Task<List<RegistrationDescription>> DeleteAllRegistrationsAsync()
        //{
        //    //TODO NB when deleting a registration you cannot use the same device Id again, so we should actually never do this in production

        //    var client = NotificationHubClient.CreateClientFromConnectionString(ConnectionString, NotificationHubPath);
        //    var registrations = await client.GetAllRegistrationsAsync(100);
        //    var count = 0;
        //    try
        //    {
                            
        //        foreach (var reg in registrations)
        //        {
        //            await client.DeleteRegistrationAsync(reg.RegistrationId);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }


        //    return registrations.ToList();
        //}

        //private static async Task CreateInstallationAsync(string pnsHandle)
        //{
        //    var result = await PushNotificationsService.UpdateDeviceRegistrationAsync(new DeviceRegistration
        //    {
        //        Platform = RuntimePlatform.Android,
        //        AccountId = "myAccountId",
        //        UserId = "myUserId",
        //        DeviceIdentifier = "myNewDeviceId",
        //        PushNotificationServiceHandle = pnsHandle,
        //        Tags = new List<string>{"RandomTaggage"},
        //        Templates = new List<IPushNotificationTemplate>
        //        {
        //            new PushNotificationTemplate("default", "Test", "Test", new List<string>
        //            {
        //                "message"
        //            })
        //        }
        //    }, CancellationToken.None);

        //}
    }
}
