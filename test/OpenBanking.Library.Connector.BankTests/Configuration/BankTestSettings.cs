// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Utility;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using Jering.Javascript.NodeJS;
using Xunit.Abstractions;

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
        public NodeJSProcessOptions NodeJSProcessOptions { get; set; } = new NodeJSProcessOptions();

        public bool NodeJSProcessOptionsAddInspectBrk { get; set; } = false;

        public OutOfProcessNodeJSServiceOptions OutOfProcessNodeJSServiceOptions { get; set; } =
            new OutOfProcessNodeJSServiceOptions();

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
        public NodeJSOptions NodeJS { get; set; } = new NodeJSOptions();

        /// <summary>
        ///     User-supplied settings which are processed in <see cref="GetProcessedPuppeteerLaunch" />.
        /// </summary>
        public PuppeteerLaunchOptions PuppeteerLaunch { get; set; } = new PuppeteerLaunchOptions();
    }

    /// <summary>
    ///     Registration to test specified by software statement profile and registration scope
    /// </summary>
    public class BankRegistrationType : IXunitSerializable
    {
        public string SoftwareStatementProfileId { get; set; } = null!;

        public RegistrationScope RegistrationScope { get; set; }

        /// <summary>
        ///     Banks to exclude from testing with this registration type.
        ///     List of banks where each bank is specified via its <see cref="BankProfiles.BankProfileEnum" /> as a string.
        /// </summary>
        public List<BankProfileEnum> ExcludedBanks { get; set; } =
            new List<BankProfileEnum>();

        public void Deserialize(IXunitSerializationInfo info)
        {
            SoftwareStatementProfileId = info.GetValue<string>(nameof(SoftwareStatementProfileId));
            RegistrationScope = info.GetValue<RegistrationScope>(nameof(RegistrationScope));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(SoftwareStatementProfileId), SoftwareStatementProfileId);
            info.AddValue(nameof(RegistrationScope), RegistrationScope);
        }

        public override string ToString()
        {
            return $"{{ SoftwareStatementProfileId = {SoftwareStatementProfileId}, Scope = {RegistrationScope} }}";
        }
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

    public class TestedBanks
    {
        /// <summary>
        ///     Banks to test in Generic Host App Tests.
        ///     List of banks where each bank is specified via its <see cref="BankProfiles.BankProfileEnum" /> as a string.
        /// </summary>
        public List<BankProfileEnum> GenericHostAppTests { get; set; } = new List<BankProfileEnum>();

        /// <summary>
        ///     Banks to test in Plain App Tests.
        ///     List of banks where each bank is specified via its <see cref="BankProfiles.BankProfileEnum" /> as a string.
        /// </summary>
        public List<BankProfileEnum> PlainAppTests { get; set; } = new List<BankProfileEnum>();
    }

    public class BankTestSettings : ISettings<BankTestSettings>
    {
        /// <summary>
        ///     Banks to test.
        /// </summary>
        public TestedBanks TestedBanks { get; set; } = new TestedBanks();

        /// <summary>
        ///     Bank registration types (clients) to test
        /// </summary>
        public List<BankRegistrationType> TestedBankRegistrationTypes { get; set; } = new List<BankRegistrationType>();

        /// <summary>
        ///     Do not allow use of "API overrides" instead of bank API call for creating bank registrations (POST /register)
        /// </summary>
        public bool ForceNewBankRegistration { get; set; } = false;

        public ConsentAuthoriserOptions ConsentAuthoriser { get; set; } = new ConsentAuthoriserOptions();

        /// <summary>
        ///     Path to data folder used for logging, "API overrides", and bank user information.
        /// </summary>
        public DataDirectory DataDirectory { get; set; } = new DataDirectory();


        /// <summary>
        ///     Log external API requests/responses. Off by default.
        /// </summary>
        public bool LogExternalApiData { get; set; } = false;

        public string SettingsSectionName => "BankTests";


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
