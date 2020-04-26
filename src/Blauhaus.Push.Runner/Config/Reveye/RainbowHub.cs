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
            pnsHandle: "https://db5p.notify.windows.com/?token=AwYAAAB1Y53x%2b0VyJCkjPkpVBkGbm9VWeAm1GQVBzsUQBjMYLsgBHABmVyXCyLKw5HgVgZV4aEG1fNiJdvv0gV8P46eZlxXq%2fsnf4mtkF11wk43d86%2f6h94WUtIr66LFiNvvC1bDKat%2bFVj3CswpkblaW%2f9%2f",
            deviceId: "8800e69d-e6af-b03b-3123-6d2b185cce3a", 
            userId: "2E911DE0-61D9-4591-B3A5-9298A644E15B")
        {
        }
    }
}