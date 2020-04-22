using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseAspersHub : BasePushRunnerHub
    {
        protected BaseAspersHub(IRuntimePlatform platform, string pnsHandle, string deviceId, string userId)
            : base(platform, pnsHandle, deviceId, userId)
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
            "e0W6iiS3p5E:APA91bHc3-nD18r7nGNM-fWe79P7AOPf1LKrYXZCWRho1ffSjwiE_5Mh1U4Eg-WtK6rUSRGbuqcOWh7c1XCyWGeQzkHs8C16T45Y8NqYfQmCrGEiACtn7tkXySsdM7cWlmysxp1Kv1_W",
            "4392199c-5458-482b-b820-1a206e965ac9",
            "A5D675FF-3F38-4FA4-9677-E30787676B0A")
        {
        }
    }

    public class AspersUwpHub : BaseAspersHub
    {
        public AspersUwpHub() : base(RuntimePlatform.UWP,
            "https://db5p.notify.windows.com/?token=AwYAAAD%2fklP4n8QQ7xvgGYsF3CErH4AFIE%2bv8MePNTqKa9pDheIZtcfq45C%2fJbC%2bQshz5Rg1bRimEw06E1pD7YK4FDl2%2bNv9WRm1cnwZgqG9X1s12E2jL5df5WDh0Pu1Emqqe6DEZsEYH3eNXFkz5w8%2f86kb",
            "5161a874-ce57-4fdd-bea6-830635673aa2",
            "A5D675FF-3F38-4FA4-9677-E30787676B0A")
        {
        }

    }

    public class AspersIosHub : BaseAspersHub
    {
        public AspersIosHub() : base(RuntimePlatform.iOS,
            "13E5A75D411DB4A8AFDA55A24473872CD2A93D466E077645994F387F613FB11A",
            "c6587b2e-383e-45cd-9073-e033ae4b2d3e",
            "A5D675FF-3F38-4FA4-9677-E30787676B0A")
        {
        }
    }
}