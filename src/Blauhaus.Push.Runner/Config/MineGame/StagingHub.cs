using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.MineGame
{
    public abstract class StagingHub : BasePushRunnerHub
    {
        protected StagingHub(IRuntimePlatform platform, string pnsHandle, string deviceId) 
            : base(platform: platform, pnsHandle: pnsHandle, deviceId: deviceId, userId: "e23bc901-4c0f-40bd-8351-7491a04b5b62")
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
            deviceId: "52dab57d459926dc")
        {
        }
    }
    
    public class StagingUwpHub : StagingHub
    {
        public StagingUwpHub() : base(platform: RuntimePlatform.UWP,
            pnsHandle: "https://par02p.notify.windows.com/?token=AwYAAAChGJaRD15gPyruP7EgG62hf8YTiJ%2fc%2bcKA66So%2fL6BIfbweLMbOtU89p19vXOVpIFjUSUKolO9tstTWkgCE%2beL3lYt4exwNi3DERTQYrigTtbc40iTGwnLOKjTeAo8QA33QkogUgTk5JigV%2fq83zDB",
            deviceId: "12caa288-e610-410c-69af-246f402a3f2a")
        {
        }
    }
    
    
    public class StagingIosHub : StagingHub
    {
        public StagingIosHub() : base(platform: RuntimePlatform.iOS,
            pnsHandle: "3D413B11591F8E2DACE0BD4A8564FB7D997AB28397487DF6587C1BA2DF6C963C",
            deviceId: "13373AEF-F73D-4E9E-927E-07079EBEE3E3")
        {
        }
    }
     
}