// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration
{
    /// <summary>
    ///     Options to be fed to "puppeteer.launch()"
    /// </summary>
    public class PlaywrightLaunchOptions
    {
        public bool Headless { get; set; } = true;

        public float SlowMo { get; set; } = 0;

        public float TimeOut { get; set; } = 0;

        public bool DevTools { get; set; } = false;

        /// <summary>
        ///     Extra parameter allowing to toggle user-specified executable path and args without constant addition/removal.
        ///     This allows for example easy switching between the user's Chrome installation plus extension(s) and
        ///     the Puppeteer version of Chromium.
        /// </summary>
        public bool IgnoreExecutablePathAndArgs { get; set; } = true;

        /// <summary>
        ///     Path to browser folder.
        /// </summary>
        public ExecutablePath ExecutablePath { get; set; } = new()
        {
            Windows = "C:/Program Files/Google/Chrome/Application/chrome.exe",
            MacOs = "~/Library/Application Support/Google/Chrome",
            Linux = "~/.config/google-chrome/default"
        };

        public string[] Args { get; set; } = Array.Empty<string>();

        public float? ProcessedSlowMo => SlowMo == 0 ? null : SlowMo;

        public string? ProcessedExecutablePath => IgnoreExecutablePathAndArgs ? null : GetExecutablePathForCurrentOs();

        public string[]? ProcessedArgs => IgnoreExecutablePathAndArgs ? null : Args;

        // Gets chrome directory for current OS platform
        public string? GetExecutablePathForCurrentOs() =>
            OsPlatformEnumHelper.GetCurrentOsPlatform() switch
            {
                OsPlatformEnum.MacOs => ExecutablePath.MacOs,
                OsPlatformEnum.Linux => ExecutablePath.Linux,
                OsPlatformEnum.Windows => ExecutablePath.Windows,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    public class EmailOptions
    {
        public string SmtpServer { get; set; } = string.Empty;

        public int SmtpPort { get; set; } = 587;

        public string FromEmailAddress { get; set; } = string.Empty;

        public string FromEmailName { get; set; } = string.Empty;

        public string FromEmailPassword { get; set; } = string.Empty;

        public string ToEmailAddress { get; set; } = string.Empty;

        public string ToEmailName { get; set; } = string.Empty;
    }

    public class ConsentAuthoriserOptions
    {
        /// <summary>
        ///     User-supplied settings which are processed in <see cref="GetProcessedPuppeteerLaunch" />.
        /// </summary>
        public PlaywrightLaunchOptions PlaywrightLaunch { get; set; } = new();

        public EmailOptions Email { get; set; } = new();
    }

    /// <summary>
    ///     Test case group specified by parameters and bank filtering rules
    /// </summary>
    public class TestGroup
    {
        /// <summary>
        ///     Software statement profile ID to use for this group of test cases
        /// </summary>
        public string SoftwareStatementProfileId { get; set; } = null!;

        /// <summary>
        ///     Bank registration scope to sue for this group of test cases
        /// </summary>
        public RegistrationScopeEnum RegistrationScope { get; set; }

        /// <summary>
        ///     Banks to test using GenericHostAppTest.
        ///     List of banks where each bank is specified via its <see cref="BankProfileEnum" /> as a string.
        /// </summary>
        public List<BankProfileEnum> GenericHostAppTests { get; set; } =
            new();

        /// <summary>
        ///     Banks to test using PlainAppTest.
        ///     List of banks where each bank is specified via its <see cref="BankProfileEnum" /> as a string.
        /// </summary>
        public List<BankProfileEnum> PlainAppTests { get; set; } =
            new();

        /// <summary>
        ///     Bank-specific overrides for software statement and certificate profiles.
        ///     Dictionary whose keys are bankProfileEnums and values are override strings.
        /// </summary>
        public Dictionary<BankProfileEnum, string>
            SoftwareStatementAndCertificateProfileOverrides { get; set; } =
            new();

        /// <summary>
        ///     Existing external (bank) API BankRegistration objects (IDs in this property) to specify when using POST to create a
        ///     new
        ///     BankRegistration
        ///     object.
        ///     Dictionary whose keys are bankProfileEnums and values are strings.
        /// </summary>
        public Dictionary<BankProfileEnum, string>
            BankRegistrationExternalApiIds { get; set; } =
            new();

        /// <summary>
        ///     Existing external (bank) API BankRegistration objects (secrets in this property) to specify when using POST to
        ///     create a new
        ///     BankRegistration
        ///     object. Secrets only used where corresponding ID specified.
        ///     Dictionary whose keys are bankProfileEnums and values are strings.
        /// </summary>
        public Dictionary<BankProfileEnum, string>
            BankRegistrationExternalApiSecrets { get; set; } =
            new();

        /// <summary>
        ///     Existing external (bank) API BankRegistration objects (registration access tokens in this property) to specify when
        ///     using POST to create a new
        ///     BankRegistration
        ///     object. Registration access tokens only used where corresponding ID specified.
        ///     Dictionary whose keys are bankProfileEnums and values are strings.
        /// </summary>
        public Dictionary<BankProfileEnum, string>
            BankRegistrationRegistrationAccessTokens { get; set; } =
            new();

        /// <summary>
        ///     Existing external (bank) API AccountAccessConsent objects (IDs in this property) to specify when using POST to
        ///     create a new
        ///     AccountAccessConsent object.
        ///     Dictionary whose keys are bankProfileEnums and values are strings.
        /// </summary>
        public Dictionary<BankProfileEnum, string>
            AccountAccessConsentExternalApiIds { get; set; } =
            new();

        /// <summary>
        ///     Existing external (bank) API AccountAccessConsent  (refresh tokens in this property)s to specify when using POST to
        ///     create a new
        ///     AccountAccessConsent object.
        ///     Dictionary whose keys are bankProfileEnums and values are strings.
        /// </summary>
        public Dictionary<BankProfileEnum, string>
            AccountAccessConsentRefreshTokens { get; set; } =
            new();

        /// <summary>
        ///     Existing external (bank) API AccountAccessConsent (auth context nonce values in this property)s to specify when
        ///     using POST to
        ///     create a new
        ///     AccountAccessConsent object.
        ///     Dictionary whose keys are bankProfileEnums and values are strings.
        /// </summary>
        public Dictionary<BankProfileEnum, string>
            AccountAccessConsentAuthContextNonces { get; set; } =
            new();
    }

    /// <summary>
    ///     Path to data folder used for logging, "API overrides", and bank user information.
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

    /// <summary>
    ///     Path to Chrome folder.
    ///     This must be set to a valid
    ///     file path for the current OS platform.
    /// </summary>
    public class ExecutablePath
    {
        // Path to chrome folder when current OS is macOS
        public string? MacOs { get; set; }

        // Path to chrome folder when current OS is Windows
        public string? Windows { get; set; }

        // Path to chrome folder when current OS is Linux
        public string? Linux { get; set; }
    }

    public class BankTestSettings : ISettings<BankTestSettings>
    {
        /// <summary>
        ///     Named groups of bank tests.
        /// </summary>
        public Dictionary<string, TestGroup> TestGroups { get; set; } = new();

        public ConsentAuthoriserOptions ConsentAuth { get; set; } = new();

        /// <summary>
        ///     Path to data folder used for logging, "API overrides", and bank user information.
        /// </summary>
        public DataDirectory DataDirectory { get; set; } = new();

        /// <summary>
        ///     Log external API requests/responses. Off by default.
        /// </summary>
        public bool LogExternalApiData { get; set; } = false;

        public string SettingsGroupName => "OpenBankingConnector:BankTests";

        public BankTestSettings Validate()
        {
            if (!Directory.Exists(GetDataDirectoryForCurrentOs()))
            {
                throw new DirectoryNotFoundException(
                    "Can't locate data path specified in bank test setting DataDirectory:" +
                    $"{GetDataDirectoryForCurrentOs()}. Please update app settings.");
            }

            // Check executable path in the case where this is not ignored
            if (!ConsentAuth.PlaywrightLaunch.IgnoreExecutablePathAndArgs)
            {
                // Check executable path is not null
                if (ConsentAuth.PlaywrightLaunch.GetExecutablePathForCurrentOs() is null)
                {
                    throw new ArgumentException("Please specify an executable path in app settings.");
                }

                // Check executable path exists
                if (!File.Exists(ConsentAuth.PlaywrightLaunch.GetExecutablePathForCurrentOs()))
                {
                    throw new DirectoryNotFoundException(
                        "Can't locate executable path specified in bank test setting ExecutablePath:" +
                        $"{ConsentAuth.PlaywrightLaunch.GetExecutablePathForCurrentOs()}. Please update app settings.");
                }
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
