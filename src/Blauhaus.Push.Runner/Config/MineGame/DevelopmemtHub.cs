using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config.MineGame
{
    public abstract class DevelopmemtHub : BasePushRunnerHub
    {
        protected DevelopmemtHub(IRuntimePlatform platform, string pnsHandle, string deviceId, string userId) 
            : base(platform: platform, pnsHandle: pnsHandle, deviceId: deviceId, userId: userId)
        {
            NotificationHubConnectionString =
                "Endpoint=sb://minegame-development-hub-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=CJ6p8lBbpOXiEfKzrIwd2dR2TSj59EIBF54b3xnNL68=";

            NotificationHubName = "minegame-development-hub";
        }

    }

    public class DevelopmentAndroidHub : DevelopmemtHub
    {
        public DevelopmentAndroidHub() : base(platform: RuntimePlatform.Android, 
            pnsHandle: "cWxlgX1vR-ihmMPF4niGLV:APA91bG7YTTrCg8PQO4J6cO319yvbJmT_0x9RHhZXIJr9X9HbkAYg0Zux5LKVl9UcWeTp_v_L1HLFXwaZnteZbyxRpdFfLajJnKMoXPMAi5ZufIKwsTvWeE-ZeMrguzninbTSenWbo7X", 
            deviceId: "3973a0b4c2e40272", 
            userId: "6da1ea78-57d1-422f-b7b3-bf160eba9be0")
        {
        }
    }
    
    public class DevelopmentUwpHub : DevelopmemtHub
    {
        public DevelopmentUwpHub() : base(platform: RuntimePlatform.UWP,
            pnsHandle: "https://par02p.notify.windows.com/?token=AwYAAAChGJaRD15gPyruP7EgG62hf8YTiJ%2fc%2bcKA66So%2fL6BIfbweLMbOtU89p19vXOVpIFjUSUKolO9tstTWkgCE%2beL3lYt4exwNi3DERTQYrigTtbc40iTGwnLOKjTeAo8QA33QkogUgTk5JigV%2fq83zDB",
            deviceId: "12caa288-e610-410c-69af-246f402a3f2a", 
            userId: "6da1ea78-57d1-422f-b7b3-bf160eba9be0")
        {
        }
    }
     
}