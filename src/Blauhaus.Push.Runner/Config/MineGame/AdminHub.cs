using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.MineGame
{
    public abstract class BaseAdminHub : BasePushRunnerHub
    {
        protected BaseAdminHub(IRuntimePlatform platform, string pnsHandle,  string deviceId, string userId) 
            : base(platform: platform, pnsHandle: pnsHandle, deviceId: deviceId, userId: userId)
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
        public AdminAndroidHub() 
            : base(platform: RuntimePlatform.Android, 
                pnsHandle: "cg5cJsN_690:APA91bFYyUXOci_9catakhvY9mrDJqrISIBhKbvE0mbIgFXN3u3_kTn-jfIWf6XtKy2P4kBAPz0eFsR0jwdF1pTfIVwYQlcWsDa6x1Ya8dilroQQuID154l2UZv-3eB5IwySx9JfNfyS",
                deviceId: "c57dc2e6f14ed0d3", 
                userId: "0FF7A478-FED1-414D-893C-E8F7694BACC0")
        {
        }
    }
    

    public class AdminUwpHub : BaseAdminHub
    {
        public AdminUwpHub() : base(platform: RuntimePlatform.UWP, pnsHandle: "",deviceId: "", userId: "")
        {
        }
    }

    public class AdminIosHub : BaseAdminHub
    {
        public AdminIosHub() : base(platform: RuntimePlatform.iOS, 
            pnsHandle: "9E577AEF37CEF080F321003C57DCA8714DE82D0C18ACEB1017F38FC7691F5AF9", 
            deviceId: "6832D9AC-A657-42B6-9FDE-64F70CAD699E", 
            userId: "0FF7A478-FED1-414D-893C-E8F7694BACC0")
        {
        }
    }
}