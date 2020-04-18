using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Game
{
    public class DevGameIos : PushRunnerHub
    {
        public DevGameIos()
        {
            NotificationHubConnectionString =
                "Endpoint=sb://minegamedevhub.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=fa802OEeFAWRmXRWTsBzXTRcHk4i3UOiwIgfj68O2Zk=;" +
                "EntityPath=minegamedevhub";
            NotificationHubName = "minegamedevhub";
            Platform = RuntimePlatform.iOS;
            DeviceId = "MyIosDeviceId";
            PnsHandle = "";
        }

    }
}