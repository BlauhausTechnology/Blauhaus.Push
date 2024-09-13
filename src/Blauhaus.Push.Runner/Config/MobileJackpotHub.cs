using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Runner.Config;

public class MobileJackpotHub : BasePushRunnerHub
{
    public MobileJackpotHub() : base(
        platform: RuntimePlatform.Android, 
        pnsHandle: "f1z423YdSEm0kxRHElGA5P:APA91bG0nYIt1TYp7tpo2Jhzy3Ynv6kA6WVJlPNm6MhxJPYelEI9_HGpbJ1q119OKa4W1KoO07Y-uHamf3RvfDuaqKAK2S2Jw7ixzNR8vxUoUh4zVkCgqRG4R8cjqqWJlIBn3S_YhLe-", 
        deviceId: "1bc0bb0585a648e4", 
        //pnsHandle: "dZaUMiXyRIqrapFMT86HxJ:APA91bHVhN-yRKX05qKrRhYwzvCRkMhNMFFZkd5uvlaQS5iOb_cxcDSEHsGoOjXuKxJZXToavXOUnFeiCeSloqnn8AjXgDLiVBAd33fzbaTMxiX_1a7JXv544HVxMpr_7y6-RSf3rZ7_", 
        //deviceId: "194782195725107a", 
        userId: "653")
    {
        NotificationHubConnectionString = "Endpoint=sb://mobile-jackpot-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=FgAFw3Mg1aEgTX8vItdZizTg/qXnO+JN6ajcbeIX7VA=";
        NotificationHubName = "mobile-jackpot-notifications";
    }
}