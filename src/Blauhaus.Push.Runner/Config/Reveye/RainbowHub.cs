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
            "dY-7NPUmQxc:APA91bF5pIKH_0iWDYk__Qc9IgNJnW2uWGu00-ymdrZvQ9PPKDev4grtCjvuczr3wTIIIPzHQvxzFU3zsggSGhwJPXu_LLyD5HHPQ9pv48evQqw5x8fKLRTO4lf0VurPO86EuwFGMjH5",
            "androidDeviceId", "")
        {
        }
    }
    
    public class RainbowUwpHub : BaseRainbowHub
    {
        public RainbowUwpHub() : base(
            platform: RuntimePlatform.UWP,
            pnsHandle: "https://am3p.notify.windows.com/?token=AwYAAABXk0fHgo%2fhFs4wvn%2fr9bBvu05pxXPhvIDHdJYLmmjoeH9T1T0e%2f51zNzCEAbj%2b7cYF75GestGA5NKiZZCienBsJ5lb1DxxbxDqtVSIBKbEgeCiHLjHA4068YiYTj1TEma3ex6tbNNB%2bkz%2fVuTX2xGT",
            deviceId: "0c81a1b1-c926-4a42-98c7-eab1e97b3140", 
            userId: "8B3A0775-973F-4442-B227-0CBBD639CA23")
        {
        }
    }
}