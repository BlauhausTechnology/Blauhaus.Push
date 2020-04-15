using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Admin
{
    public class DevAdminIosSandbox : PushRunnerConfig
    {
        public DevAdminIosSandbox()
        {
            NotificationHubConnectionString =
                    "Endpoint=sb://minegamedevadminiossandbox.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=kDIla2IiV9ESJWsmoyWbse4i9dGq2HlVA2dIaNH1m/k=;" +
                    "EntityPath=minegamedevadminiossandbox";
            NotificationHubName = "minegamedevadmin";
            Platform = RuntimePlatform.iOS;
            DeviceId = "MyIosDeviceId";
            PnsHandle = "";
        }

    }
}