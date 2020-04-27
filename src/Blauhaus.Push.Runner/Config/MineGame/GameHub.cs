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
            pnsHandle: "cmuEVxOUh9I:APA91bGN91H-GbNIgToTKQgldPdSeczzGJZbX8Z_od3PgMG0YK9XkqQP7DWgMhJPTnRbD3jXB8IVQufoMI9hbNy34opvjCqmG33CFwSyIqieaf-nIvMHuWOvN8Ul8Msx6Z1EpXpCNtOV", 
            deviceId: "52dab57d459926dc", 
            userId: "0FF7A478-FED1-414D-893C-E8F7694BACC0")
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
            pnsHandle: "", deviceId: "", userId: "")
        {
        }
    }
}