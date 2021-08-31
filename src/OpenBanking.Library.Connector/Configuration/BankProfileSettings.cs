// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    /// <summary>
    ///     Path to data directory used for hidden bank bank profile properties.
    ///     This must be set to a valid
    ///     file path for the current OS platform.
    ///     This path should not be in the public repo to ensure this data is not committed there.
    /// </summary>
    public class DataDirectory
    {
        // Path to data folder when current OS is macOS
        public string MacOs { get; set; } = "";

        // Path to data folder when current OS is Windows
        public string Windows { get; set; } = "";

        // Path to data folder when current OS is Linux
        public string Linux { get; set; } = "";
    }

    public class BankProfileSettings : ISettings<BankProfileSettings>
    {
        /// <summary>
        ///     Path to data directory used for hidden bank bank profile properties.
        /// </summary>
        public DataDirectory DataDirectory { get; set; } = new DataDirectory();

        public string HiddenPropertiesFile => Path.Combine(
            GetDataDirectoryForCurrentOs(),
            "bankProfileHiddenProperties.json");

        public string SettingsSectionName => "BankProfiles";

        public BankProfileSettings Validate()
        {
            if (!Directory.Exists(GetDataDirectoryForCurrentOs()))
            {
                throw new DirectoryNotFoundException(
                    "Can't locate directory " +
                    $"{GetDataDirectoryForCurrentOs()} from BankProfiles:DataDirectory setting. Please update app settings.");
            }

            if (!File.Exists(HiddenPropertiesFile))
            {
                throw new FileNotFoundException(
                    "Can't locate bankProfileHiddenProperties.json file in BankProfiles:DataDirectory setting" +
                    $"{GetDataDirectoryForCurrentOs()}. Please update app settings.");
            }

            return this;
        }

        // Gets data directory for current OS platform
        public string GetDataDirectoryForCurrentOs() =>
            OsPlatformEnumHelper.GetCurrentOsPlatform() switch
            {
                OsPlatformEnum.MacOs => DataDirectory.MacOs,
                OsPlatformEnum.Linux => DataDirectory.Linux,
                OsPlatformEnum.Windows => DataDirectory.Windows,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
