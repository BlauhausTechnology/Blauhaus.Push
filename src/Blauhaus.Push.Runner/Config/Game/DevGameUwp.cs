using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Game
{
    public class DevGameUwp : PushRunnerConfig
    {
        public DevGameUwp()
        {
            NotificationHubConnectionString =
                "Endpoint=sb://minegamedevhub.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=fa802OEeFAWRmXRWTsBzXTRcHk4i3UOiwIgfj68O2Zk=;" +
                "EntityPath=minegamedevhub";
            NotificationHubName = "minegamedevhub";
            Platform = RuntimePlatform.UWP;
            DeviceId = "MyUwpDeviceId";
            PnsHandle = "";
        }

    }
}