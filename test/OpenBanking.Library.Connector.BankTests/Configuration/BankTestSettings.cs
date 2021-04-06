// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using Jering.Javascript.NodeJS;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration
{
    /// <summary>
    ///     Selection of parameters of "puppeteer.launch()" exposed for customisation, can add to this list as
    ///     desired.
    /// </summary>
    public class PuppeteerLaunchOptions
    {
        public bool? Headless { get; set; } = true;

        public string[]? Args { get; set; } = new string[0];

        public string? ExecutablePath { get; set; }

        public double? SlowMo { get; set; } = null;

        /// <summary>
        ///     Extra parameter allowing to toggle user-specified executable path and args without constant addition/removal.
        ///     This allows for example easy switching between the user's Chrome installation plus extension(s) and
        ///     the Puppeteer version of Chromium.
        /// </summary>
        public bool IgnoreExecutablePathAndArgs { get; set; } = false;
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
            new OutOfProcessNodeJSServiceOptions
            {
                NumRetries = 0,
            };

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

        /// <summary>
        ///     Get processed version of <see cref="PuppeteerLaunch" />
        /// </summary>
        /// <returns></returns>
        public PuppeteerLaunchOptions GetProcessedPuppeteerLaunch()
        {
            PuppeteerLaunchOptions options = PuppeteerLaunch;
            if (options.IgnoreExecutablePathAndArgs)
            {
                options.ExecutablePath = null;
                options.Args = null;
            }

            return options;
        }
    }

    public class BankWhitelistSettings
    {
        /// <summary>
        ///     Whitelist of banks to test in .NET Generic Host app tests for each software statement profile.
        ///     Dictionary whose key is the ID of a software statement profile and whose value is a whitelist
        ///     of banks where each bank is specified via its <see cref="BankProfiles.BankProfileEnum" /> as a string.
        ///     All banks will be tested for any software statement profile that doesn't have a whitelist.
        /// </summary>
        public Dictionary<string, List<BankProfileEnum>> GenericHostAppTests { get; set; } =
            new Dictionary<string, List<BankProfileEnum>>();

        /// <summary>
        ///     Whitelist of banks to test in "plain" app tests for each software statement profile.
        ///     Dictionary whose key is the ID of a software statement profile and whose value is a whitelist
        ///     of banks where each bank is specified via its <see cref="BankProfiles.BankProfileEnum" /> as a string.
        ///     All banks will be tested for any software statement profile that doesn't have a whitelist.
        /// </summary>
        public Dictionary<string, List<BankProfileEnum>> PlainAppTests { get; set; } =
            new Dictionary<string, List<BankProfileEnum>>();
    }

    /// <summary>
    ///     Registration to test specified by software statement profile and registration scope
    /// </summary>
    public class BankRegistrationType : IXunitSerializable
    {
        public string SoftwareStatementProfileId { get; set; } = null!;

        public RegistrationScope RegistrationScope { get; set; }

        public void Deserialize(IXunitSerializationInfo info)
        {
            SoftwareStatementProfileId = info.GetValue<string>(nameof(SoftwareStatementProfileId));
            RegistrationScope = info.GetValue<RegistrationScope>(nameof(RegistrationScope));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(SoftwareStatementProfileId), SoftwareStatementProfileId);
            info.AddValue(nameof(RegistrationScope), SoftwareStatementProfileId);
        }

        public override string ToString()
        {
            return $"{{ SoftwareStatementProfileId = {SoftwareStatementProfileId}, Scope = {RegistrationScope} }}";
        }
    }

    public class BankTestSettings : ISettings<BankTestSettings>
    {
        /// <summary>
        ///     Bank registrations (clients) to test
        /// </summary>
        public List<BankRegistrationType> TestedBankRegistrationTypes { get; set; } = new List<BankRegistrationType>();

        /// <summary>
        ///     Whitelists of banks to be tested for each app type and software statement
        /// </summary>
        public BankWhitelistSettings BankWhitelists { get; set; } = new BankWhitelistSettings();

        public ConsentAuthoriserOptions ConsentAuthoriser { get; set; } = new ConsentAuthoriserOptions();

        public bool UseInMemoryDatabase { get; set; } = true;

        public string SettingsSectionName => "BankTests";

        public BankTestSettings Validate()
        {
            return this;
        }
    }
}
