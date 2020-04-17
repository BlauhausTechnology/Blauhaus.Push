using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseAspersConfig : PushRunnerConfig
    {
        protected BaseAspersConfig(IRuntimePlatform platform, string pnsHandle)
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

    public class AspersAndroidConfig : BaseAspersConfig
    {
        public AspersAndroidConfig() : base(RuntimePlatform.Android, 
                "fEBRy1cvWn4:APA91bFxA2L48sFcNFeOAuGuVs7JRcprVeuRAmWZFczMxoIDD1DR6Ahlyf8uF8mMHbm8VY_QtYda1sOUPbp3y5pz_MLfTUMyCwGU-M2ISYMlzmQKRraW-GFMnJ7ObKOUPVYLBcsiQMhM")
        {
        }
    }

    
    public class AspersUwpConfig : BaseAspersConfig
    {
        public AspersUwpConfig() : base(RuntimePlatform.UWP,
                "https://db5p.notify.windows.com/?token=AwYAAACVwSK3x81cjeQRa3IdEl35tsQ3c2fvE5kHneBz41%2bqNUVBhhUtfHc2EDyPYBkw039%2bIHtaow7DylWTLJqMevwu2Y5tGBBrGd%2f56JJ6yT5S9Hj3kPmBFY5T8hB6zucuS5ozcjqWyvGBIPlT49idqAxj")
        {
        }
    }
}