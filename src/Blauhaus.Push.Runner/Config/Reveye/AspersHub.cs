using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseAspersHub : PushRunnerHub
    {
        protected BaseAspersHub(IRuntimePlatform platform, string pnsHandle)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://reveye-push.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=mk4xOnfkwdbOAZ7CvAyAX0FwgOTm8TJFisEhj6hdhOE=;" +
                "EntityPath=reveye-push-aspers";

            NotificationHubName = "reveye-push-aspers";
            
            DeviceId = platform.Value + "_deviceId";
            Platform = platform;
            PnsHandle = pnsHandle;
        }

    }

    public class AspersAndroidHub : BaseAspersHub
    {
        public AspersAndroidHub() : base(RuntimePlatform.Android, 
            "dY-7NPUmQxc:APA91bF5pIKH_0iWDYk__Qc9IgNJnW2uWGu00-ymdrZvQ9PPKDev4grtCjvuczr3wTIIIPzHQvxzFU3zsggSGhwJPXu_LLyD5HHPQ9pv48evQqw5x8fKLRTO4lf0VurPO86EuwFGMjH5")
        {
        }
    }

    
    public class AspersUwpHub : BaseAspersHub
    {
        public AspersUwpHub() : base(RuntimePlatform.UWP,
                "https://db5p.notify.windows.com/?token=AwYAAACVwSK3x81cjeQRa3IdEl35tsQ3c2fvE5kHneBz41%2bqNUVBhhUtfHc2EDyPYBkw039%2bIHtaow7DylWTLJqMevwu2Y5tGBBrGd%2f56JJ6yT5S9Hj3kPmBFY5T8hB6zucuS5ozcjqWyvGBIPlT49idqAxj")
        {
        }
    }
}