using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.MineGame
{
    public abstract class BaseGameHub : BasePushRunnerHub
    {
        protected BaseGameHub(IRuntimePlatform platform, string pnsHandle, string deviceId, string userId) 
            : base(platform, pnsHandle, deviceId, userId)
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
        public GameAndroidHub() : base(RuntimePlatform.Android, 
            "", "", "")
        {
        }
    }
    
    public class GameUwpHub : BaseGameHub
    {
        public GameUwpHub() : base(RuntimePlatform.UWP,
            "https://db5p.notify.windows.com/?token=AwYAAAANTjlKv%2bwU0%2f%2bnWdzDmWTMWnZuXzwrqecvO3fioWdBonwAqLygADjWzJXXd8AJwyD0t%2fbtTmgdMSbN762Guytann%2fmPxjlRRmMZkdi3fpfyAp1vYm1LuRzhvaG8Tb9CbYGecEGrjcPCFG3Xbyn4LHJ", "", "")
        {
        }
    }

    public class GameIosHub : BaseGameHub
    {
        public GameIosHub() : base(RuntimePlatform.iOS,
            "", "", "")
        {
        }
    }
}