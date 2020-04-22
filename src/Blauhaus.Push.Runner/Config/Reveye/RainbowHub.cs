using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseRainbowHub : BasePushRunnerHub
    {
        protected BaseRainbowHub(IRuntimePlatform platform, string pnsHandle,  string deviceId, string userId) 
            : base(platform, pnsHandle, deviceId, userId)
        {
            NotificationHubConnectionString = 
                "Endpoint=sb://reveye-push.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=6JvabD86nZm2+LRVyDs67r/T1X5gc8SebLDs6pU7FVw=;" +
                "EntityPath=reveye-push-rainbow";

            NotificationHubName = "reveye-push-rainbow";
        }
    }

    public class RainbowAndroidHub : BaseRainbowHub
    {
        public RainbowAndroidHub() : base(RuntimePlatform.Android, 
            "fzILCXtAU3M:APA91bGDmwp7YH4OiLPyxYOd1L6U8W8-bPZhPF3lmc2AELh5HW71Yfbawl4N-6RIRx7lgAsTf2DD0djWcl23ZHgQepez1AMoq0m_AvVOHk-fFeJQy5c5jRxysI0RSqLZM6c2CTQE7NOx",
            "", "")
        {
        }
    }
    
    public class RainbowUwpHub : BaseRainbowHub
    {
        public RainbowUwpHub() : base(
            platform: RuntimePlatform.UWP,
            pnsHandle: "https://am3p.notify.windows.com/?token=AwYAAABXk0fHgo%2fhFs4wvn%2fr9bBvu05pxXPhvIDHdJYLmmjoeH9T1T0e%2f51zNzCEAbj%2b7cYF75GestGA5NKiZZCienBsJ5lb1DxxbxDqtVSIBKbEgeCiHLjHA4068YiYTj1TEma3ex6tbNNB%2bkz%2fVuTX2xGT",
            deviceId: "", 
            userId: "")
        {
        }
    }
}