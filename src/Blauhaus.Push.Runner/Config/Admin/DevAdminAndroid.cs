using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Admin
{
    public class DevAdminAndroid : PushRunnerConfig
    {
        public DevAdminAndroid()
        {
            NotificationHubConnectionString =
                "Endpoint=sb://minegamedevadmin.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=6ULhJh6mDzlRr6r0dZ2T6dL8MZHHve10nMq2V36c2T0=;" +
                "EntityPath=minegamedevadmin";
            NotificationHubName = "minegamedevadmin";
            Platform = RuntimePlatform.Android;
            DeviceId = "MyAndroidDeviceId";
            PnsHandle = "";
        }

    }
}