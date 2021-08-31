// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace FinnovationLabs.OpenBanking.Library.Connector.Utility
{
    // Supported OS platforms enum
    public enum OsPlatformEnum
    {
        MacOs,
        Linux,
        Windows
    }

    // Helper class for OsPlatformEnum
    public static class OsPlatformEnumHelper
    {
        static OsPlatformEnumHelper()
        {
            AllOsPlatforms = Enum.GetValues(typeof(OsPlatformEnum)).Cast<OsPlatformEnum>();
        }

        public static IEnumerable<OsPlatformEnum> AllOsPlatforms { get; }


        // Return true if current OS platform matches input
        public static bool IsCurrentPlatform(OsPlatformEnum osPlatform) =>
            osPlatform switch
            {
                OsPlatformEnum.MacOs => RuntimeInformation.IsOSPlatform(OSPlatform.OSX),
                OsPlatformEnum.Linux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
                OsPlatformEnum.Windows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
                _ => throw new ArgumentOutOfRangeException(nameof(osPlatform), osPlatform, null)
            };

        // Get current OS platform
        public static OsPlatformEnum GetCurrentOsPlatform()
        {
            foreach (OsPlatformEnum osPlatform in AllOsPlatforms)
            {
                if (IsCurrentPlatform(osPlatform))
                {
                    return osPlatform;
                }
            }

            throw new InvalidOperationException("No OS platform matches current platform.");
        }
    }
}
