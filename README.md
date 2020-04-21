# Blauhaus.Push

A wrapper around Azure Notification Hubs for projects using .NET Core server-side and Xamarin.Forms on the client. 

## Azure Setup

When setting up the Notification Hub on Azure, note the following:
- Costs are shared between all hubs created in a given namespace
- The free tier does not support sending directly to devices, so you will need to use the template method on the Free tier. 

Once your hub is set up, go to Access policies and copy the values for *DefaultFullSharedAccessSignature* and *DefaultListenSharedAccessSignature*. You will use these in your config. 

## ANDROID

### Cloud

Azure Notification Hubs uses Firebase Cloud Messaging (FCM) under the hood, so you need to go to the FCM portal first. 

After creating a project, click the Android icon under "Get started by adding Firebase to your app". Enter the package id (bund identifier) for your app and click Register App.

Under "Project Settings" in the FCM portal, select Cloud Messaging and copy the Server Key. Enter this in the Azure Notification Hub Google (GCM / FCM) section. 

### Device

Install Xamarin.Firebase.Messaging and Blauhaus.Push.Client from nuget. 

Download the google-services.json file from your Android app settings on FCM and put it in the root of your Android app project – it must have the GoogleJsonServices build action. If you don't see the build action, install all the relevant nuget packages and restart Visual Studio. 

#### AndroidManifest.xml

Ensure that the AndroidManifest.xml file includes the following permissions:

<uses-permission android:name="com.google.android.c2dm.permission.RECEIVE" />
<uses-permission android:name="android.permission.WAKE_LOCK" />
<uses-permission android:name="android.permission.GET_ACCOUNTS"/>

Add the following between the <application></application> tags in the AndroidManifest.xml file:

<receiver android:name="com.google.firebase.iid.FirebaseInstanceIdInternalReceiver" android:exported="false" />
<receiver android:name="com.google.firebase.iid.FirebaseInstanceIdReceiver" android:exported="true" android:permission="com.google.android.c2dm.permission.SEND">
    <intent-filter>
    <action android:name="com.google.android.c2dm.intent.RECEIVE" />
    <action android:name="com.google.android.c2dm.intent.REGISTRATION" />
    <category android:name="${applicationId}" />
    </intent-filter>
</receiver>

#### PushService.cs

Add a class to the Android project called PushService.cs. It must get a reference to the AndroidPushNotificationHandler from the Ioc Container, and invoke the handlers for new tokens and new messages:

[Service]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
public class PushService : FirebaseMessagingService
{
    public override void OnNewToken(string token)
    {
        base.OnNewToken(token);

        GetAReferenceTo<AndroidPushNotificationHandler>()
            .HandleNewTokenAsync(token);
    }

    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);

        GetAReferenceTo<AndroidPushNotificationHandler>()
            .HandleNewTokenAsync(token);
    }
}

#### MainActivity.cs

Ensure that the MainActivity has the following attributes:

MainLauncher = true, 
LaunchMode = LaunchMode.SingleTop,
NoHistory = true

In OnCreate, add a call to Initialize the Android handler:

protected override void OnCreate(Bundle savedInstanceState)
{
    base.OnMessageReceived(message);

    GetAReferenceTo<AndroidPushNotificationHandler>()
        .Initialize(this, Intent, (NotificationManager)GetSystemService(NotificationService));
}

NB this doesn't seem to work when there is a separate splash screen activity. There must only be one Activity, set to be SingleTop. If you need a different theme for app startup, use the splash screen theme for the MainActivity (Theme = "@style/MainTheme.Splash") and then switch to the app theme in OnCreate using SetTheme(Resource.Style.MainTheme).

## UWP

### Cloud

### Device


## iOS

### Cloud

### Device
