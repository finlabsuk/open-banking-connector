// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Jering.Javascript.NodeJS;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration
{
    /// <summary>
    ///     Options to be fed to "puppeteer.launch()"
    /// </summary>
    public class PuppeteerLaunchOptions
    {
        public bool Headless { get; set; } = true;

        public double? SlowMo { get; set; } = null;

        public bool DevTools { get; set; } = false;

        /// <summary>
        ///     Extra parameter allowing to toggle user-specified executable path and args without constant addition/removal.
        ///     This allows for example easy switching between the user's Chrome installation plus extension(s) and
        ///     the Puppeteer version of Chromium.
        /// </summary>
        public bool IgnoreExecutablePathAndArgs { get; set; } = false;


        /// <summary>
        ///     Path to chrome folder.
        /// </summary>
        public ExecutablePath? ExecutablePath { get; set; }

        public string[] Args { get; set; } = new string[0];

        public PuppeteerLaunchOptionsJavaScript ToJavaScript()
        {
            var js = new PuppeteerLaunchOptionsJavaScript { Headless = Headless, SlowMo = SlowMo, Devtools = DevTools };

            //if using executable path then retrieve path using the GetExecutablePath method
            if (!IgnoreExecutablePathAndArgs)
            {
                js.ExecutablePath = GetExecutablePathForCurrentOs();
                js.Args = Args;
            }

            return js;
        }

        // Gets chrome directory for current OS platform
        public string? GetExecutablePathForCurrentOs() =>
            OsPlatformEnumHelper.GetCurrentOsPlatform() switch
            {
                OsPlatformEnum.MacOs => ExecutablePath?.MacOs,
                OsPlatformEnum.Linux => ExecutablePath?.Linux,
                OsPlatformEnum.Windows => ExecutablePath?.Windows,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    /// <summary>
    ///     Processed version of <see cref="PuppeteerLaunchOptions" /> which is sent to JavaScript.
    /// </summary>
    public class PuppeteerLaunchOptionsJavaScript
    {
        public bool? Headless { get; set; }

        public double? SlowMo { get; set; }

        public bool? Devtools { get; set; }

        public string? ExecutablePath { get; set; }

        public string[]? Args { get; set; }
    }

    /// <summary>
    /// </summary>
    public class NodeJSOptions
    {
        /// <summary>
        ///     User-supplied settings which are processed in <see cref="GetProccessedNodeJSProcessOptions" />.
        /// </summary>
        public NodeJSProcessOptions NodeJSProcessOptions { get; set; } = new();

        public bool NodeJSProcessOptionsAddInspectBrk { get; set; } = false;

        public OutOfProcessNodeJSServiceOptions OutOfProcessNodeJSServiceOptions { get; set; } = new();

        /// <summary>
        ///     Get processed version of <see cref="NodeJSProcessOptions" />
        /// </summary>
        /// <returns></returns>
        public NodeJSProcessOptions GetProccessedNodeJSProcessOptions()
        {
            NodeJSProcessOptions nodeJSNodeJSProcessOptions = NodeJSProcessOptions;
            if (NodeJSProcessOptionsAddInspectBrk)
            {
                nodeJSNodeJSProcessOptions.NodeAndV8Options = string.Join(
                    " ",
                    nodeJSNodeJSProcessOptions.NodeAndV8Options,
                    "--inspect-brk");
            }

            nodeJSNodeJSProcessOptions.ProjectPath = FolderPaths.ConsentAuthoriserBuildFolder;

            return nodeJSNodeJSProcessOptions;
        }
    }

    public class ConsentAuthoriserOptions
    {
        public NodeJSOptions NodeJS { get; set; } = new();

        /// <summary>
        ///     User-supplied settings which are processed in <see cref="GetProcessedPuppeteerLaunch" />.
        /// </summary>
        public PuppeteerLaunchOptions PuppeteerLaunch { get; set; } = new();
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
        ///     BankRegistration objects to POST instead of POSTing the default request object.
        ///     Dictionary whose keys are bankProfileEnums and values are BankRegistration objects
        /// </summary>
        public Dictionary<BankProfileEnum, BankRegistration>
            BankRegistrationObjects { get; set; } =
            new();

        /// <summary>
        ///     Existing BankRegistration objects to use instead of using POST to create a new object.
        ///     Dictionary whose keys are bankProfileEnums and values are GUID IDs
        /// </summary>
        public Dictionary<BankProfileEnum, Guid>
            BankRegistrationIds { get; set; } =
            new();

        /// <summary>
        ///     Existing AccountAccessConsent objects to use instead of using POST to create a new object.
        ///     Dictionary whose keys are bankProfileEnums and values are GUID IDs
        /// </summary>
        public Dictionary<BankProfileEnum, Guid>
            AccountAccessConsentIds { get; set; } =
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
        ///     Groups of bank tests.
        /// </summary>
        public List<TestGroup> TestGroups { get; set; } = new();

        public ConsentAuthoriserOptions ConsentAuthoriser { get; set; } = new();

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
            if (!ConsentAuthoriser.PuppeteerLaunch.IgnoreExecutablePathAndArgs)
            {
                // Check executable path is not null
                if (ConsentAuthoriser.PuppeteerLaunch.GetExecutablePathForCurrentOs() is null)
                {
                    throw new ArgumentException("Please specify an executable path in app settings.");
                }

                // Check executable path exists
                if (!File.Exists(ConsentAuthoriser.PuppeteerLaunch.GetExecutablePathForCurrentOs()))
                {
                    throw new DirectoryNotFoundException(
                        "Can't locate executable path specified in bank test setting ExecutablePath:" +
                        $"{ConsentAuthoriser.PuppeteerLaunch.GetExecutablePathForCurrentOs()}. Please update app settings.");
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
