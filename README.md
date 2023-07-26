# Blauhaus.Push

A wrapper around Azure Notification Hubs for projects using .NET Core server-side and Xamarin.Forms on the client. 

## Azure Setup

When setting up the Notification Hub on Azure, note the following:
- Costs are shared between all hubs created in a given namespace
- The free tier does not support sending directly to devices, so you will need to use the template method on the Free tier. 

Once your hub is set up, go to Access policies and copy the values for *DefaultFullSharedAccessSignature* and *DefaultListenSharedAccessSignature*. You will use these in your config. 

## Server Setup

Install Blauhaus.Push.Server and add push notification services to the service collection using the .AddPushNotificationsServer() extension method. 

Configure an implementation of IPushNotificationsHub containing the NotificationHubName you created on Azure, and the DefaultFullSharedAccessSignature connection string.
NB you need to add the notification hub name to the connection string as the EntityPah, eg "{connectionStringFromAzure};EntityPath={notificationHubName}".

Once you get a PushNotificationServiceHandle from the device, send it to the server and call UpdateDeviceRegistrationAsync, passing in the PnsHandle and other info, eg:

```c#
await _pushNotificationsServerService.UpdateDeviceRegistrationAsync(new DeviceRegistration
{
    PushNotificationServiceHandle = userDevice.PushNotificationServiceHandle,
    AccountId = "",
    UserId = userDevice.UserId,
    DeviceIdentifier = userDevice.DeviceIdentifier,
    Platform = userDevice.Platform,
    Templates = new List<IPushNotificationTemplate>
    {
        Templates.Message //this is a default template
    }
}, hub, token);
```

To send a message to a user, call _pushNotificationsServerService.SendNotificationToUserAsync(), passing in their userId and a PushNotification.

When a user logs out, call _pushNotificationsServerService.DeregisterUserDeviceAsync with the userId and deviceId you used to register this user's device.

## Device Setup

In the .NET Standard class library, add a PushConfig class that implements `IPushNotificationsClientConfig`. Give it the NotificationHubName you created on Azure, and the DefaultListenSharedAccessSignature connection string.
NB you need to add the notification hub name to the connection string as the EntityPah, eg "{connectionStringFromAzure};EntityPath={notificationHubName}"

Register the push services and config with the serviceCollection using:
```c#
services.AddPushNotificationsClient<MoonbasePushConfig>();
```

If you want to handle notifications that are tapped, add an implementation of `IPushNotificationTapHandler` and register it using:
```c#
services.AddPushNotificationsClient<MoonbasePushConfig, TapHandlerImplementation>();
```

## ANDROID

### Cloud

Azure Notification Hubs uses Firebase Cloud Messaging (FCM) under the hood, so you need to go to the FCM portal first. 

After creating a project, click the Android icon under "Get started by adding Firebase to your app". Enter the package id (bund identifier) for your app and click Register App.

Under "Project Settings" in the FCM portal, select Cloud Messaging and copy the Server Key. 
If it is disabled click the 3 dots, select "Manage API in Google Cloud Console" and click the enable button there - the server key should now be available. 
Enter the server key in the Azure Notification Hub Google (GCM / FCM) section. 

### Device

Install Xamarin.Firebase.Messaging and Blauhaus.Push.Client.Maui from nuget:

```xml
<!--Project references-->
<ItemGroup>
	<ProjectReference Include="..\Lumen.Maui\Lumen.Maui.csproj" />
</ItemGroup>
	
<!--Android packages-->
<ItemGroup Condition="'$(TargetFramework)' == 'net7.0-android'">
	<PackageReference Include="Xamarin.Firebase.Messaging">
		<Version>123.1.2.2</Version>
	</PackageReference> 
</ItemGroup>
```

At the moment it also looks like we have to install Xamarin.Google.Dagger:
https://stackoverflow.com/questions/64428377/java-lang-nosuchmethoderror-no-static-method-checkbuilderrequirement

Download the google-services.json file from your Android app settings on FCM and put it in the root of your Android app project – it must have the GoogleJsonServices build action. If you don't see the build action, install all the relevant nuget packages and restart Visual Studio. 

#### AndroidManifest.xml

Ensure that the AndroidManifest.xml file includes the following permissions:

```xml
<uses-permission android:name="com.google.android.c2dm.permission.RECEIVE" />
<uses-permission android:name="android.permission.WAKE_LOCK" />
<uses-permission android:name="android.permission.GET_ACCOUNTS"/>
```

Add the following between the <application></application> tags in the AndroidManifest.xml file:

```xml
<receiver android:name="com.google.firebase.iid.FirebaseInstanceIdInternalReceiver" android:exported="false" />
<receiver android:name="com.google.firebase.iid.FirebaseInstanceIdReceiver" android:exported="true" android:permission="com.google.android.c2dm.permission.SEND">
    <intent-filter>
    <action android:name="com.google.android.c2dm.intent.RECEIVE" />
    <action android:name="com.google.android.c2dm.intent.REGISTRATION" />
    <category android:name="${applicationId}" />
    </intent-filter>
</receiver>
```

#### PushService.cs

Add a class to the Android project called PushService.cs. It must get a reference to the AndroidPushNotificationHandler from the Ioc Container, and invoke the handlers for new tokens and new messages:

```c#
[Service]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
public class PushService : FirebaseMessagingService
{
    public override void OnNewToken(string token)
    {
        base.OnNewToken(token);

        AppServiceLocator.Resolve<AndroidPushNotificationHandler>()
            .HandleNewTokenAsync(token);
    }

    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);

        AppServiceLocator.Resolve<AndroidPushNotificationHandler>()
            .HandleMessageReceived(message);
    }
}
````

#### MainActivity.cs

Ensure that the MainActivity has the following attributes:

```
MainLauncher = true, 
LaunchMode = LaunchMode.SingleTop,
NoHistory = true
```

In OnCreate, add a call to Initialize the Android handler:

```c#
protected override void OnCreate(Bundle savedInstanceState)
{
    base.OnCreate(savedInstanceState);

    AppServiceLocator.Resolve<AndroidPushNotificationHandler>()
        .Initialize(this, Intent, (NotificationManager)GetSystemService(NotificationService));
}
```
NB this doesn't seem to work when there is a separate splash screen activity. There must only be one Activity, set to be SingleTop. If you need a different theme for app startup, use the splash screen theme for the MainActivity (Theme = "@style/MainTheme.Splash") and then switch to the app theme in OnCreate using SetTheme(Resource.Style.MainTheme).

Override OnNewIntent:

```c#
 protected override void OnNewIntent(Intent intent)
{
    base.OnNewIntent(intent);
            
    AppServiceLocator.Resolve<AndroidPushNotificationHandler>()
        .HandleNewIntent(intent);
}
```

#### Service registration

Finally, register the Android dependencies with the Service Collection using services.AddAndroidPushNotifications();


## iOS

### Cloud

* On a Mac, generate a Certificate Signing Request using the KeyChain Access Certificate Assistant, and save it to disk
* On the Apple Developer portal, ensure the App Id is enabled for push notifications, and click the Configure button next to the Push capability option.
* Create a production certificate using the CSR created on the Mac
* Download the cert on the mac, double-click it to add to keychain, and then export a .p12
* Upload the p12 cert to bitrise
* Go to Notification Hub on Azure and select Apple (APNS) from the menu
* Upload the p12 file and enter password
* NB you have to recreate any existing provisioning profiles after adding push capabilities. The old ones will not have the aps-environment key in them. 
* To enable testing locally on iOS you need to create an additional cert and add it to the app identifier as a development APNS cert. Then create an alternative Entitlements.plist for debugging that has  <string>development</string>

### Device

#### AppDelegate.cs

In FinishedLaunching, after the call to LoadApplication, call 

```c#
AppServiceLocator.Resolve<IosPushNotificationHandler>().InitializeAsync(this, options);
```

Add overrides for notification event handlers:

```c#
public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
{
    AppServiceLocator.Resolve<IosPushNotificationHandler>()
        .HandleNewToken(deviceToken);
}

public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
{
    AppServiceLocator.Resolve<IosPushNotificationHandler>()
        .HandleFailedRegistration(error);
}

public override void ReceivedRemoteNotification(UIApplication application, NSDictionary options)
{
    AppServiceLocator.Resolve<IosPushNotificationHandler>()
        .HandleMessageReceivedAsync(application.ApplicationState, options);
}
```

#### Entitlements.plist:

```xml
<key>aps-environment</key>
<string>production</string>
```

#### Service registration

Register the iOS dependencies with the Service Collection using services.AddIosPushNotifications();


## UWP

### Cloud

* Go to the Dev Center and create an app. 
* Under product management > WNS / MPNS > click the Live Services link
* Copy the SID and application secret, and enter them in Azure portal under Windows (WNS)
(NB this has changed! - there is no Live Services link any more and the link they give you to "App Registration portal" on azure doesn't seem to generate valid secrets)


### Device

In Visual Studio, associate the app with the store. 

#### App.xaml.cs

In OnLaunched, just before Window.Current.Activate(), call:
 await AppServiceLocator.Resolve<UwpPushNotificationHandler>().HandleAppLaunching(e);

#### MainPage.xaml.cs
After LoadApplication, call:
Task.Run(async ()=> await AppServiceLocator.Resolve<UwpPushNotificationHandler>().InitializeAsync()); 

#### Service registration

Register the Uwp dependencies with the Service Collection using services.AddUwpPushNotifications();





