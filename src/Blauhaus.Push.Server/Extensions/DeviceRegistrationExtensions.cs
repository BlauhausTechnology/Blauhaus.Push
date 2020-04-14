using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Service;
using CSharpFunctionalExtensions;

namespace Blauhaus.Push.Server.Extensions
{
    public static class DeviceRegistrationExtensions
    {
        public static bool IsNotValid(this IDeviceRegistration deviceRegistration, object sender, IAnalyticsService analyticsService, out string error)
        {

            if (deviceRegistration == null)
            {
                error = analyticsService.TraceError(sender, PushErrors.InvalidDeviceRegistration);
                return true;
            }

            if (deviceRegistration.Templates.Count == 0)
            {
                error = analyticsService.TraceError(sender, PushErrors.NoTemplateProvidedOnRegistration, deviceRegistration.ToObjectDictionary());
                return true;
            }

            if (string.IsNullOrEmpty(deviceRegistration.PushNotificationServiceHandle))
            {
                error = analyticsService.TraceError(sender, PushErrors.InvalidPnsHandle, deviceRegistration.ToObjectDictionary());
                return true;
            }

            if (deviceRegistration.Platform == null || string.IsNullOrEmpty(deviceRegistration.Platform.Value) || 
                deviceRegistration.Platform.Value != RuntimePlatform.iOS.Value && 
                deviceRegistration.Platform.Value != RuntimePlatform.Android.Value &&
                deviceRegistration.Platform.Value != RuntimePlatform.UWP.Value)
            {
                error = analyticsService.TraceError(sender, PushErrors.InvalidPlatform);
                return true;
            }

            if (string.IsNullOrEmpty(deviceRegistration.DeviceIdentifier))
            {
                deviceRegistration.DeviceIdentifier = Guid.NewGuid().ToString();
                analyticsService.TraceVerbose(sender, "No DeviceIdentifier received, generating new InstallationId");
            }

            foreach (var template in deviceRegistration.Templates)
            {
                foreach (var templateDataProperty in template.DataProperties)
                {
                    var reservedStrings = ReservedStrings.GetForPlatform(deviceRegistration.Platform);
                    if (reservedStrings.Contains(templateDataProperty))
                    {
                        error = analyticsService.TraceError(sender, PushErrors.ReservedString(templateDataProperty));
                        return true;
                    }
                }
            }

            error = string.Empty;
            return false;
        }
         
    }
}