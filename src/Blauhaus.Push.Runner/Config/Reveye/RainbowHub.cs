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
            "dw82chUi43Q:APA91bFgwyv-mqaJbmexleSBFY0NAN1vEski9myxvwWKcM-EeIGgWXA2-_RsJQnzXQ2i_9chFQB69KcDnTvAmxbN4UDwMvSfahlFIxw6T7BAls4aYQiX8dNaHjAaKhxh_LMpN0C_EcZr",
            "", "")
        {
        }
    }
    
    public class RainbowIosHub : BaseRainbowHub
    {
        public RainbowIosHub() : base(RuntimePlatform.iOS, 
            "69A848A52708D9602AFB8DF72B14C691C7038627A4EE967D5E25ECA150AEBBB5",
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