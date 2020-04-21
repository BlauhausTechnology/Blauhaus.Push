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
            "dY-7NPUmQxc:APA91bF5pIKH_0iWDYk__Qc9IgNJnW2uWGu00-ymdrZvQ9PPKDev4grtCjvuczr3wTIIIPzHQvxzFU3zsggSGhwJPXu_LLyD5HHPQ9pv48evQqw5x8fKLRTO4lf0VurPO86EuwFGMjH5",
            "ddba760d-d0c8-484b-b716-817d32199453", 
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
}