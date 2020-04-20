using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseRainbowHub : BasePushRunnerHub
    {
        protected BaseRainbowHub(IRuntimePlatform platform, string pnsHandle) 
            : base(platform, pnsHandle)
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
            "dY-7NPUmQxc:APA91bF5pIKH_0iWDYk__Qc9IgNJnW2uWGu00-ymdrZvQ9PPKDev4grtCjvuczr3wTIIIPzHQvxzFU3zsggSGhwJPXu_LLyD5HHPQ9pv48evQqw5x8fKLRTO4lf0VurPO86EuwFGMjH5")
        {
        }
    }
    
    public class RainbowUwpHub : BaseRainbowHub
    {
        public RainbowUwpHub() : base(RuntimePlatform.UWP,
                "https://db5p.notify.windows.com/?token=AwYAAACVwSK3x81cjeQRa3IdEl35tsQ3c2fvE5kHneBz41%2bqNUVBhhUtfHc2EDyPYBkw039%2bIHtaow7DylWTLJqMevwu2Y5tGBBrGd%2f56JJ6yT5S9Hj3kPmBFY5T8hB6zucuS5ozcjqWyvGBIPlT49idqAxj")
        {
        }
    }
}