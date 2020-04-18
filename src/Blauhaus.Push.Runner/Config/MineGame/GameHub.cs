﻿using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.MineGame
{
    public abstract class BaseGameHub : BasePushRunnerHub
    {
        protected BaseGameHub(IRuntimePlatform platform, string pnsHandle) 
            : base(platform, pnsHandle)
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
            "")
        {
        }
    }
    
    public class GameUwpHub : BaseGameHub
    {
        public GameUwpHub() : base(RuntimePlatform.UWP,
                "")
        {
        }
    }

    public class GameIosHub : BaseGameHub
    {
        public GameIosHub() : base(RuntimePlatform.iOS,
            "")
        {
        }
    }
}