using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.MineGame
{
    public abstract class AdminSandboxHub : BasePushRunnerHub
    {
        protected AdminSandboxHub(string pnsHandle, string deviceId) 
            : base(RuntimePlatform.iOS, pnsHandle, deviceId)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://minegamedevadminiossandbox.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=kDIla2IiV9ESJWsmoyWbse4i9dGq2HlVA2dIaNH1m/k=" +
                "EntityPath=minegamedevadminiossandbox ";

            NotificationHubName = "minegamedevadminiossandbox ";
        }

    }


    public class AdminSandboxIosHub : AdminSandboxHub
    {
        public AdminSandboxIosHub() : base("", "")
        {
        }
    }
}