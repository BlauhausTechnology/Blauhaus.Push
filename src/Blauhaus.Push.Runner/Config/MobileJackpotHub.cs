using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config;

public class MobileJackpotHub : BasePushRunnerHub
{
    public MobileJackpotHub() : base(
        platform: RuntimePlatform.Android, 
        pnsHandle: "d3qJ5-2cTM-sUG9NfZe-vx:APA91bHjU0niuFut_DwnY2sdFaYhJkdyUHsTR8VHjC9rqFg2UPgusOgWahH4WAbS-xtPXSxk8ImTjhaXOKTNaaVyfc3luqnaeqcRvAVq2MesBSGN8N9AxxlQetPGL-0GCbK7WyIXLfX0", 
        deviceId: "1bc0bb0585a648e4", 
        userId: "653")
    {
        NotificationHubConnectionString = "Endpoint=sb://mobile-jackpot-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=FgAFw3Mg1aEgTX8vItdZizTg/qXnO+JN6ajcbeIX7VA=";
        NotificationHubName = "mobile-jackpot-notifications";
    }
}