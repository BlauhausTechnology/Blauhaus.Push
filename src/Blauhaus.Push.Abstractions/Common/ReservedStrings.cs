using System.Collections.Generic;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Abstractions.Common
{
    public static class ReservedStrings
    {

        public static List<string> GetForPlatform(IRuntimePlatform platform)
        {
            if (platform.Value == RuntimePlatform.UWP.Value)
                return Uwp;
            if (platform.Value == RuntimePlatform.Android.Value)
                return Android;
            if (platform.Value == RuntimePlatform.iOS.Value)
                return Ios;

            return new List<string>();
        }

        public static readonly List<string> Uwp = new List<string>
        {
            "body",
            "title",

            "%22"
        };
        
        public static readonly List<string> Android = new List<string>
        {
            "body",
            "title",

            "data",
        };
        
        public static readonly List<string> Ios = new List<string>
        {
            "body",
            "title",

            "aps"
        };

    }
}