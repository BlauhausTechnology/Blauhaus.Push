using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Errors;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Server;
using Microsoft.Extensions.Logging;

namespace Blauhaus.Push.Server.Extensions
{
    public static class DeviceRegistrationExtensions
    {
        public static bool IsNotValid(this IDeviceRegistration? deviceRegistration, object sender, IAnalyticsLogger logger, out Error error)
        {

            if (deviceRegistration == null)
            {
                error = PushErrors.InvalidDeviceRegistration;
                logger.LogError(error);
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
                logger.LogWarning("Invalid DeviceRegistration: {@DeviceRegistration}", deviceRegistration);
                logger.LogError(error); 
                return true;
            }

            if (deviceRegistration.Platform == null || string.IsNullOrEmpty(deviceRegistration.Platform.Value) || 
                deviceRegistration.Platform.Value != RuntimePlatform.iOS.Value && 
                deviceRegistration.Platform.Value != RuntimePlatform.Android.Value &&
                deviceRegistration.Platform.Value != RuntimePlatform.UWP.Value)
            {
                error = PushErrors.InvalidPlatform;
                logger.LogWarning("Invalid DeviceRegistration: {@DeviceRegistration}", deviceRegistration);
                logger.LogError(error); 
                return true;
            }

            if (string.IsNullOrEmpty(deviceRegistration.DeviceIdentifier))
            {
                error = PushErrors.MissingDeviceIdentifier;
                logger.LogWarning("Invalid DeviceRegistration: {@DeviceRegistration}", deviceRegistration);
                logger.LogError(error); 
                return true;
            }
            
            if (string.IsNullOrEmpty(deviceRegistration.UserId))
            {
                error = PushErrors.MissingUserId;
                logger.LogWarning("Invalid DeviceRegistration: {@DeviceRegistration}", deviceRegistration);
                logger.LogError(error); 
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
                        logger.LogWarning("Invalid DeviceRegistration: {@DeviceRegistration}", deviceRegistration);
                        logger.LogError(error); 
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