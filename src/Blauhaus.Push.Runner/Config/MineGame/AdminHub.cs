using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.MineGame
{
    public abstract class BaseAdminHub : BasePushRunnerHub
    {
        protected BaseAdminHub(IRuntimePlatform platform, string pnsHandle,  string deviceId, string userId) 
            : base(platform, pnsHandle, deviceId, userId)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://minegamedevadmin.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=6ULhJh6mDzlRr6r0dZ2T6dL8MZHHve10nMq2V36c2T0=;" +
                "EntityPath=minegamedevadmin";

            NotificationHubName = "minegamedevadmin";
        }

    }

    public class AdminAndroidHub : BaseAdminHub
    {
        public AdminAndroidHub() : base(RuntimePlatform.Android, "", "", "")
        {
        }
    }
    
    public class AdminUwpHub : BaseAdminHub
    {
        public AdminUwpHub() : base(RuntimePlatform.UWP, "","", "")
        {
        }
    }

    public class AdminIosHub : BaseAdminHub
    {
        public AdminIosHub() : base(RuntimePlatform.iOS, "blah-blah", "", "")
        {
        }
    }
}