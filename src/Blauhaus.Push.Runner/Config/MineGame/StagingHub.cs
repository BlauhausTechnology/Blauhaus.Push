using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.MineGame
{
    public abstract class StagingHub : BasePushRunnerHub
    {
        protected StagingHub(IRuntimePlatform platform, string pnsHandle, string deviceId, string userId) 
            : base(platform: platform, pnsHandle: pnsHandle, deviceId: deviceId, userId: userId)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://minegame-staging-hub-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=FlBdlcSCnmiPBaCGrlcHr3+xSdmZrTNFSmfigcEKk7I=";

            NotificationHubName = "minegame-staging-hub";
        }

    }

    public class StagingAndroidHub : StagingHub
    {
        public StagingAndroidHub() : base(platform: RuntimePlatform.Android, 
            pnsHandle: "em9ToA0eTaywSFm0SfhHvo:APA91bFshHduxDMzWirejTBZd8_kMzG-e9WJJe3dm-vulWtHWXey8Sa0D9yRsOKJ2BqBQVrmICanM8hEMGpRI3zbPTTXnfVun14zp_lTEaqW54kg6DGV-YHFEUkngnojdnQu4GXGnYDJ", 
            deviceId: "52dab57d459926dc", 
            userId: "e23bc901-4c0f-40bd-8351-7491a04b5b62")
        {
        }
    }
    
    public class StagingUwpHub : StagingHub
    {
        public StagingUwpHub() : base(platform: RuntimePlatform.UWP,
            pnsHandle: "https://par02p.notify.windows.com/?token=AwYAAAChGJaRD15gPyruP7EgG62hf8YTiJ%2fc%2bcKA66So%2fL6BIfbweLMbOtU89p19vXOVpIFjUSUKolO9tstTWkgCE%2beL3lYt4exwNi3DERTQYrigTtbc40iTGwnLOKjTeAo8QA33QkogUgTk5JigV%2fq83zDB",
            deviceId: "12caa288-e610-410c-69af-246f402a3f2a", 
            userId: "e23bc901-4c0f-40bd-8351-7491a04b5b62")
        {
        }
    }
     
}