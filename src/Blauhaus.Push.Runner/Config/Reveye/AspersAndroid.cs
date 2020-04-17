using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public class AspersAndroid: PushRunnerConfig
    {
        public AspersAndroid()
        {
            NotificationHubConnectionString =
                "Endpoint=sb://reveye-push.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=mk4xOnfkwdbOAZ7CvAyAX0FwgOTm8TJFisEhj6hdhOE=;" +
                "EntityPath=reveye-push-aspers";
            NotificationHubName = "reveye-push-aspers";
            Platform = RuntimePlatform.Android;
            DeviceId = "MyAndroidDeviceId";
            PnsHandle = "fEBRy1cvWn4:APA91bFxA2L48sFcNFeOAuGuVs7JRcprVeuRAmWZFczMxoIDD1DR6Ahlyf8uF8mMHbm8VY_QtYda1sOUPbp3y5pz_MLfTUMyCwGU-M2ISYMlzmQKRraW-GFMnJ7ObKOUPVYLBcsiQMhM";
        }
    }
}