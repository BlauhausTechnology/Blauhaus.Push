using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseBuckysHub : BasePushRunnerHub
    {
        protected BaseBuckysHub(IRuntimePlatform platform, string pnsHandle, string deviceId, string userId) 
            : base(platform, pnsHandle, deviceId, userId)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://reveye-push.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=Q9Tqvo+hqBx4XuTR/dwMfp2jSQtR0x9kGA6krkvKCK4=;" +
                "EntityPath=reveye-push-buckys";

            NotificationHubName = "reveye-push-buckys";
        }
    }

    public class BuckysAndroidHub : BaseBuckysHub
    {
        public BuckysAndroidHub() : base(RuntimePlatform.Android, 
            "dY-7NPUmQxc:APA91bF5pIKH_0iWDYk__Qc9IgNJnW2uWGu00-ymdrZvQ9PPKDev4grtCjvuczr3wTIIIPzHQvxzFU3zsggSGhwJPXu_LLyD5HHPQ9pv48evQqw5x8fKLRTO4lf0VurPO86EuwFGMjH5",
            "androidDeviceId", "")
        {
        }
    }
    
    public class BuckysUwpHub : BaseBuckysHub
    {
        public BuckysUwpHub() : base(RuntimePlatform.UWP,
                "https://db5p.notify.windows.com/?token=AwYAAACVwSK3x81cjeQRa3IdEl35tsQ3c2fvE5kHneBz41%2bqNUVBhhUtfHc2EDyPYBkw039%2bIHtaow7DylWTLJqMevwu2Y5tGBBrGd%2f56JJ6yT5S9Hj3kPmBFY5T8hB6zucuS5ozcjqWyvGBIPlT49idqAxj",
                "uwpDeviceId", "")
        {
        }
    }
}