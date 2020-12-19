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
                "SharedAccessKey=fa802OEeFAWRmXRWTsBzXTRcHk4i3UOiwIgfj68O2Zk=";

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
            pnsHandle: "https://par02p.notify.windows.com/?token=AwYAAADzGHlLVDfebz6LYjZcHBB7%2bVL1IhNib9bsTzpAgCBw7%2fzJubu3C6KL1z39IzZYNZsEwQ2Tp5hDw%2beXfpJ3K4MNckAPfUT55WhtR6GhpAbZraIkDuLPgpOd9rAPg%2fNNmza%2bp0AmaqnUNgtgUnSWpuGe",
            deviceId: "201f8114-070d-b6b8-b1ba-47f289ca3471", 
            userId: "0ff7a478-fed1-414d-893c-e8f7694bacc0")
        {
        }
    }

    public class GameIosHub : BaseGameHub
    {
        public GameIosHub() : base(platform: RuntimePlatform.iOS,
            pnsHandle: "084632978D726A132158027A084550CA2D97C4B0D157F602F1CFB39D1897EB3F", 
            deviceId: "13373AEF-F73D-4E9E-927E-07079EBEE3E3",
            userId: "0ff7a478-fed1-414d-893c-e8f7694bacc0")
        {
        }
    }
}