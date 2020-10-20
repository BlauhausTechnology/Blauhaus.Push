using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Errors;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Server;

namespace Blauhaus.Push.Server.Extensions
{
    public static class DeviceRegistrationExtensions
    {
        public static bool IsNotValid(this IDeviceRegistration deviceRegistration, object sender, IAnalyticsService analyticsService, out Error error)
        {

            if (deviceRegistration == null)
            {
                error = PushErrors.InvalidDeviceRegistration;
                analyticsService.TraceError(sender, error);
                return true;
            }

            //if (deviceRegistration.Templates.Count == 0)
            //{
            //    error = analyticsService.TraceError(sender, PushErrors.NoTemplateProvidedOnRegistration, deviceRegistration.ToObjectDictionary());
            //    return true;
            //}

            if (string.IsNullOrEmpty(deviceRegistration.PushNotificationServiceHandle))
            {
                error = PushErrors.InvalidPnsHandle;
                analyticsService.TraceError(sender, error, deviceRegistration.ToObjectDictionary()); 
                return true;
            }

            if (deviceRegistration.Platform == null || string.IsNullOrEmpty(deviceRegistration.Platform.Value) || 
                deviceRegistration.Platform.Value != RuntimePlatform.iOS.Value && 
                deviceRegistration.Platform.Value != RuntimePlatform.Android.Value &&
                deviceRegistration.Platform.Value != RuntimePlatform.UWP.Value)
            {
                error = PushErrors.InvalidPlatform;
                analyticsService.TraceError(sender, error, deviceRegistration.ToObjectDictionary()); 
                return true;
            }

            if (string.IsNullOrEmpty(deviceRegistration.DeviceIdentifier))
            {
                error = PushErrors.MissingDeviceIdentifier;
                analyticsService.TraceError(sender, error, deviceRegistration.ToObjectDictionary());  
                return true;
            }
            
            if (string.IsNullOrEmpty(deviceRegistration.UserId))
            {
                error = PushErrors.MissingUserId;
                analyticsService.TraceError(sender, error, deviceRegistration.ToObjectDictionary()); 
                return true;
            }

            foreach (var template in deviceRegistration.Templates)
            {
                foreach (var templateDataProperty in template.DataProperties)
                {
                    var reservedStrings = ReservedStrings.GetForPlatform(deviceRegistration.Platform);
                    if (reservedStrings.Contains(templateDataProperty))
                    {
                        error = PushErrors.ReservedString(templateDataProperty);
                        analyticsService.TraceError(sender, error, deviceRegistration.ToObjectDictionary());  
                        return true;
                    }
                }
            }

            error = Error.None;
            return false;
        }

        public static string ExtractInstallationId(this IDeviceRegistration deviceRegistration)
        {
            return deviceRegistration.UserId + "___" + deviceRegistration.DeviceIdentifier;
        }
    }
}