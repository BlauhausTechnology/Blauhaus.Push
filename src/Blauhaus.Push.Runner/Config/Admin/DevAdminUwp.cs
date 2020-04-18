using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Admin
{
    public class DevAdminUwp : PushRunnerHub
    {
        public DevAdminUwp()
        {
            NotificationHubConnectionString =
                "Endpoint=sb://minegamedevadmin.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=6ULhJh6mDzlRr6r0dZ2T6dL8MZHHve10nMq2V36c2T0=;" +
                "EntityPath=minegamedevadmin";
            NotificationHubName = "minegamedevadmin";
            Platform = RuntimePlatform.UWP;
            DeviceId = "MyUwpDeviceId";
            PnsHandle = "";
        }

    }
}