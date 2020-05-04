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
            pnsHandle: "https://db5p.notify.windows.com/?token=AwYAAACJgDUQ3FxgbcjlRwYd3AVHohKpA8E7KUyIOPpuTlp908tmb%2fLezHBx1Iv%2fI%2fl3igSoPcFNdAM0SHiVrJJiRdXbLzPMRiJIFzPUlMYaaiwyeqId7R%2bAnKHu9UwwjGgo37SBAaVjLf7lAakafGqutbGe",
            deviceId: "", 
            userId: "2E911DE0-61D9-4591-B3A5-9298A644E15B")
        {
        }
    }
}