using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.MineGame
{
    public abstract class BaseGameHub : BasePushRunnerHub
    {
        protected BaseGameHub(IRuntimePlatform platform, string pnsHandle, string deviceId, string userId) 
            : base(platform: platform, pnsHandle: pnsHandle, deviceId: deviceId, userId: userId)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://minegamedevhub.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=fa802OEeFAWRmXRWTsBzXTRcHk4i3UOiwIgfj68O2Zk=;" +
                "EntityPath=minegamedevhub";

            NotificationHubName = "minegamedevhub";
        }

    }

    public class GameAndroidHub : BaseGameHub
    {
        public GameAndroidHub() : base(platform: RuntimePlatform.Android, 
            pnsHandle: "fT5D5IsSbSs:APA91bFonZgtP6OSSTuu1ej8zxvDVGTec_4wOD6pIUYt2Dd9UbAmSDU7tdbttAiONCnd7hpoMVPevKVg3scx_VEGKp01CPCmFtcBgSe3V9kfNbEebz97iVMIAdgFqoUgIPQqIpgGFaMz", 
            deviceId: "c57dc2e6f14ed0d3", 
            userId: "4DE82274-DD70-4C34-833A-2BE2AB36FEB1")
        {
        }
    }
    
    public class GameUwpHub : BaseGameHub
    {
        public GameUwpHub() : base(platform: RuntimePlatform.UWP,
            pnsHandle: "https://db5p.notify.windows.com/?token=AwYAAAANKf9P63pukiLqul3HjYwjNbl8DKloqVq5foqihBT%2bz1OeIZl2gmoObwtF7lFT%2bEO%2fxqfUVIxVO6WvZU2hFFsgSP%2bpc%2fNStW8fv34TNkg8dBqpzcTh7kuhKzy15kV88se%2f2B%2f6%2b%2b4ODju3BRJVgE2W",
            deviceId: "201f8114-070d-b6b8-b1ba-47f289ca3471", 
            userId: "0FF7A478-FED1-414D-893C-E8F7694BACC0")
        {
        }
    }

    public class GameIosHub : BaseGameHub
    {
        public GameIosHub() : base(platform: RuntimePlatform.iOS,
            pnsHandle: "CCA1ED6C170AB011147C37D3BCC151F0DD0C7E731566D99FACBC7BCDE4A925C2", 
            deviceId: "6832D9AC-A657-42B6-9FDE-64F70CAD699E",
            userId: "D23BCCEA-DD4F-460E-B7D6-F542CC4808EA")
        {
        }
    }
}