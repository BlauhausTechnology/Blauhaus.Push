using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseAspersHub : BasePushRunnerHub
    {
        protected BaseAspersHub(IRuntimePlatform platform, string pnsHandle, string deviceId) 
            : base(platform, pnsHandle, deviceId)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://reveye-push.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=mk4xOnfkwdbOAZ7CvAyAX0FwgOTm8TJFisEhj6hdhOE=;" +
                "EntityPath=reveye-push-aspers";

            NotificationHubName = "reveye-push-aspers";
        }
    }

    public class AspersAndroidHub : BaseAspersHub
    {
        public AspersAndroidHub() : base(RuntimePlatform.Android, 
            "dY-7NPUmQxc:APA91bF5pIKH_0iWDYk__Qc9IgNJnW2uWGu00-ymdrZvQ9PPKDev4grtCjvuczr3wTIIIPzHQvxzFU3zsggSGhwJPXu_LLyD5HHPQ9pv48evQqw5x8fKLRTO4lf0VurPO86EuwFGMjH5",
            "androidDeviceId")
        {
        }
    }
    
    public class AspersUwpHub : BaseAspersHub
    {
        public AspersUwpHub() : base(RuntimePlatform.UWP,
                "https://am3p.notify.windows.com/?token=AwYAAACWeMP3sweDZb4rsYLl1jmqHCChpXn%2buihtBr446nOT5Fbe1YL3qxp7pEDN1a%2bvUxqNUybF7cZkI5BG%2fdVxf%2bEUVmGPcZCcQpeGGVIXqXH3ayxmn4bOHNHV7NvebF61nmmwtJnVwdd0ggBhHQvt9JWi",
                "5161a874-ce57-4fdd-bea6-830635673aa2")
        {
        }
    }
}