using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Moonbase
{
    public abstract class StagingHub : BasePushRunnerHub
    {
        protected StagingHub(IRuntimePlatform platform, string pnsHandle, string deviceId) 
            : base(platform: platform, pnsHandle: pnsHandle, deviceId: deviceId, userId: "04e21f11-174c-4970-b9a3-193615d25873")
        {
            NotificationHubConnectionString =
                "Endpoint=sb://moonbase-staging-hub-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=JOczSUq4pT98SzUkfD+xtLsgyxmp0jcac9gZFaGJ+hw=";

            NotificationHubName = "moonbase-staging-hub";
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
            pnsHandle: "https://par02p.notify.windows.com/?token=AwYAAAAgGVhSzkgIpmIsJo9jXA10rQwKHA8Wxs0FYJ26tiSz1sKrWWIaUpCaszRJTe1dsFivsvvTwWRcEdwhmGA8nwfoogJNepbbtqFcVYL3Lo9cXIwVOicmOc%2bI36jKJqW%2bnOC0UGxVi21szOo758feWEs3",
            deviceId: "201f8114-070d-b6b8-b1ba-47f289ca3471")
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