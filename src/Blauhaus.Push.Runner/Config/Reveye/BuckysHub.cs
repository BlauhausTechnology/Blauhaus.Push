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
                pnsHandle: "https://db5p.notify.windows.com/?token=AwYAAAAEk4LUFdOxtqYaniGYyIK6fleFm0NmmCpbLqEayBReCgHC%2fPmzPtBVoEhjwEc3mw45yKLiWRt%2beaiGF%2fdFUOvPYt0blkpInNYNZONiFca%2fY6ehUR95Gw2HKTwmakpPRo%2bbkucSOjBq%2bnAvyVEktSU0",
                deviceId: "", 
                userId: "92C5D5A6-B015-4FDE-ACD9-2655C6B21D13")
        {
        }
    }
}