using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.Reveye
{
    public abstract class BaseBuckysHub : BasePushRunnerHub
    {
        protected BaseBuckysHub(IRuntimePlatform platform, string pnsHandle, string deviceId, string userId) 
            : base(platform: platform, pnsHandle: pnsHandle, deviceId: deviceId, userId: userId)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://reveye-push.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=Q9Tqvo+hqBx4XuTR/dwMfp2jSQtR0x9kGA6krkvKCK4=;" +
                "EntityPath=reveye-push-buckys";

            NotificationHubName = "reveye-push-buckys";
        }
    }

    public class BuckysAndroidHub : BaseBuckysHub
    {
        public BuckysAndroidHub() : base(platform: RuntimePlatform.Android, 
            pnsHandle: "e8xB-7bA6hM:APA91bFcTnhOBvpesclqwDZZZB5FgL8JuQOzykK2yCeFmxUyfcRiH-HSiJY7ssiPACrstlxB4arPTRz4kQhY3M_DAkeFDHlllKjN0wRp1wGazw06W2xTDsLIzPZ3wYflFfV-bHSQfaWp",
            deviceId: "", 
            userId: "92C5D5A6-B015-4FDE-ACD9-2655C6B21D13")
        {
        }
    }
    
    public class BuckysIosHub : BaseBuckysHub
    {
        public BuckysIosHub() : base(platform: RuntimePlatform.iOS, 
            pnsHandle: "41C34D7D31B1C1919A3895ACB9CFD7BC99731657D9A916C390AB07904C3173F9",
            deviceId: "",
            userId: "00EE52D3-B122-44E7-9349-D17B720E980F")
        {
        }
    }
    
    public class BuckysUwpHub : BaseBuckysHub
    {
        public BuckysUwpHub() : base(platform: RuntimePlatform.UWP,
                pnsHandle: "https://am3p.notify.windows.com/?token=AwYAAADpHWcmCYOfO1iJtYF8H%2fu9fBlRQSgwbsV3wp3BkDgkBibB46HC19Htmh63vsmvofw0UEVAPSnUi0mGEai6A0oWcUyXzdj7y2aA7IQ4u0PGmmCD4zo4XFP2I%2f5KriR0SxnondtS2W%2bj11xwZgSFCae5",
                deviceId: "", userId: "")
        {
        }
    }
}