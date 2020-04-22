using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseAspersSandboxHub : BasePushRunnerHub
    {
        protected BaseAspersSandboxHub(string pnsHandle, string deviceId, string userId) 
            : base(RuntimePlatform.iOS, pnsHandle, deviceId, userId)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://reveye-push.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=hW7El3JxcDZxsedQeWYuwlXQDiElU+uWDSzOGIo+cLM=;" +
                "EntityPath=reveye-push-aspers-ios-sandbox";

            NotificationHubName = "reveye-push-aspers-ios-sandbox";
        }
    }
     
    public class AspersIosSandboxHub : BaseAspersSandboxHub
    {
        public AspersIosSandboxHub() : base(
            pnsHandle: "26566FAFDA8F6AB0D37E061C904C0F781443D4A40B756DD3592E6C5559E14600",
            deviceId: "93437915-F831-4684-B47B-1AE20ECDD5D8", 
            userId: "A5D675FF-3F38-4FA4-9677-E30787676B0A")
        {
        }
    }
}