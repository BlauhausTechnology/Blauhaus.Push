using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseRainbowHub : BasePushRunnerHub
    {
        protected BaseRainbowHub(IRuntimePlatform platform, string pnsHandle,  string deviceId) 
            : base(platform, pnsHandle, deviceId)
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
            "androidDeviceId")
        {
        }
    }
    
    public class RainbowUwpHub : BaseRainbowHub
    {
        public RainbowUwpHub() : base(RuntimePlatform.UWP,
                "https://db5p.notify.windows.com/?token=AwYAAAB%2f5BljxmKpDxReeld49LaxtNecQOnI0Zj1rAIg1nQ2C41%2b95JmMHTzm4mXnzNYNtAdUt7LsiynQ6g5pZq2dMmBXbpljslQfdgP2qomX4fxZEqTa2r1FrJvKFjwPBjicPmJuh7MOEtDoLuE5rtYD0Jq",
                "5376462f-9061-4629-bf65-0cc048f9ee84")
        {
        }
    }
}