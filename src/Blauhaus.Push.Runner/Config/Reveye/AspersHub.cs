using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseAspersHub : BasePushRunnerHub
    {
        protected BaseAspersHub(IRuntimePlatform platform, string pnsHandle, string deviceId)
            : base(platform, pnsHandle, deviceId, "A5D675FF-3F38-4FA4-9677-E30787676B0A")
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
        public AspersAndroidHub() : base(platform: RuntimePlatform.Android,
            pnsHandle: "dMef6M0RiZI:APA91bEjCxTWnJDg9cjuoCCV22LixpuW8Kf05x7CCvxgVbUE38TRZEh9BbAT4OkltcwMUq2zCOtPjR1-4CkIv8vX3US-Ge2jd0OGQUC6CKTj5-roXjc1e_CEezX1s6MVCe8KRk-dIrqz",
            deviceId: "")
        {
        }
    }

    public class AspersUwpHub : BaseAspersHub
    {
        public AspersUwpHub() : base(RuntimePlatform.UWP,
            "https://db5p.notify.windows.com/?token=AwYAAAD%2fklP4n8QQ7xvgGYsF3CErH4AFIE%2bv8MePNTqKa9pDheIZtcfq45C%2fJbC%2bQshz5Rg1bRimEw06E1pD7YK4FDl2%2bNv9WRm1cnwZgqG9X1s12E2jL5df5WDh0Pu1Emqqe6DEZsEYH3eNXFkz5w8%2f86kb",
            "")
        {
        }

    }

    public class AspersIosHub : BaseAspersHub
    {
        public AspersIosHub() : base(RuntimePlatform.iOS,
            "13E5A75D411DB4A8AFDA55A24473872CD2A93D466E077645994F387F613FB11A",
            "")
        {
        }
    }
}