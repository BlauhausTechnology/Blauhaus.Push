﻿using Blauhaus.Common.ValueObjects.Errors;

namespace Blauhaus.Push.Abstractions
{
    public static class PushErrors
    {
        public static readonly Error InvalidDeviceRegistration = Error.Create("DeviceRegistration is required");
        public static readonly Error InvalidPnsHandle = Error.Create("The PushNotificationServiceHandle provided by the device operating system is required to register a device for push notifications");
        public static readonly Error InvalidPlatform = Error.Create("The requested RuntimePlatform is not supported. Only Android, UWP and iOS are currently allowed");
        public static readonly Error NoTemplateProvidedOnRegistration = Error.Create("At least one Template must be provided when registering a device for push notifications");
        public static readonly Error RegistrationDoesNotExist = Error.Create("The requested device registration does not exist");
        public static Error ReservedString(string value) => Error.Create($"The string \"{value}\" is reserved for internal use. Please use something else");
    }
}