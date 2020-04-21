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
                "https://am3p.notify.windows.com/?token=AwYAAADpHWcmCYOfO1iJtYF8H%2fu9fBlRQSgwbsV3wp3BkDgkBibB46HC19Htmh63vsmvofw0UEVAPSnUi0mGEai6A0oWcUyXzdj7y2aA7IQ4u0PGmmCD4zo4XFP2I%2f5KriR0SxnondtS2W%2bj11xwZgSFCae5",
                "9acadc82-f0c2-4e8b-95c7-11862f2d035b", 
                "569AF4F2-9144-4D8F-96A2-C55CF3E44C30")
        {
        }
    }
}