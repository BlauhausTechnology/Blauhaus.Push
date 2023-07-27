using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Lumen
{
    public abstract class LumenDevelopmemtHub : BasePushRunnerHub
    {
        protected LumenDevelopmemtHub(IRuntimePlatform platform, string pnsHandle, string deviceId, string userId) 
            : base(platform: platform, pnsHandle: pnsHandle, deviceId: deviceId, userId: userId)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://lumen-devtest-push.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=k+AVYoRwTuet1mLUPEu5TDJPX7xD8mcRnZluwFplNyo=";

            NotificationHubName = "lumen-development-push";
        }

    }

    public class LumenDevelopmentAndroidHub : LumenDevelopmemtHub
    {
        public LumenDevelopmentAndroidHub() : base(platform: RuntimePlatform.Android, 
            pnsHandle: "daDkPIwvSMuu64W7EqvR7c:APA91bE7DhMmaoCR0P3aCMDm9bdVTwqrw1TVASRbcKjZI8DeUSRLzRjgW0m7hM1OPfSqx49qqty2rU-LNLvK_7LQQIIVNagY7q7jjHZeT6A-UVo2tr-S7XtqsNS0mbM420iETtr8Ov0B", 
            deviceId: "74818c51-1308-4ccf-b1c9-00352222ea98", 
            userId: "04160000-29c6-1297-f996-08db72f9dd79")
        {
        }
    }
    
    public class DevelopmentUwpHub : LumenDevelopmemtHub
    {
        public DevelopmentUwpHub() : base(platform: RuntimePlatform.UWP,
            pnsHandle: "https://par02p.notify.windows.com/?token=AwYAAAChGJaRD15gPyruP7EgG62hf8YTiJ%2fc%2bcKA66So%2fL6BIfbweLMbOtU89p19vXOVpIFjUSUKolO9tstTWkgCE%2beL3lYt4exwNi3DERTQYrigTtbc40iTGwnLOKjTeAo8QA33QkogUgTk5JigV%2fq83zDB",
            deviceId: "12caa288-e610-410c-69af-246f402a3f2a", 
            userId: "6da1ea78-57d1-422f-b7b3-bf160eba9be0")
        {
        }
    }
     
}