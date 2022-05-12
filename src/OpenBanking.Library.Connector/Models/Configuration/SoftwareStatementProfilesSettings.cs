// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    public class SoftwareStatementProfile
    {
        /// <summary>
        ///     Whether profile is active or inactive (ignored by Open Banking Connector). This allows profiles to be "switched on
        ///     and off" for testing etc.
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        ///     Software statement assertion (SSA) as string, i.e. "FirstPart.SecondPart.ThirdPart".
        /// </summary>
        public string SoftwareStatement { get; set; } = string.Empty;

        /// <summary>
        ///     ID of <see cref="TransportCertificateProfile" /> to use for mutual TLS with this software statement profile.
        /// </summary>
        public string TransportCertificateProfileId { get; set; } = string.Empty;

        /// <summary>
        ///     ID of <see cref="SigningCertificateProfile" /> to use for signing JWTs etc with this software statement profile.
        /// </summary>
        public string SigningCertificateProfileId { get; set; } = string.Empty;

        /// <summary>
        ///     Default redirect URL for consent authorisation when OAuth2 response_mode = fragment.
        /// </summary>
        public string DefaultFragmentRedirectUrl { get; set; } = string.Empty;
    }

    /// <summary>
    ///     Software statement profile provided to Open Banking Connector as part of
    ///     <see cref="SoftwareStatementProfilesSettings" />.
    ///     This class captures a software statement and associated data such as keys and certificates.
    /// </summary>
    public class SoftwareStatementProfileWithOverrideProperties : SoftwareStatementProfile
    {
        /// <summary>
        ///     Bank-specific overrides for
        ///     <see cref="SoftwareStatementProfile.TransportCertificateProfileId" />
        /// </summary>
        public Dictionary<string, string> TransportCertificateProfileIdOverrides { get; set; } =
            new();

        /// <summary>
        ///     Bank-specific overrides for
        ///     <see cref="SoftwareStatementProfile.SigningCertificateProfileId" />
        /// </summary>
        public Dictionary<string, string> SigningCertificateProfileIdOverrides { get; set; } =
            new();

        /// <summary>
        ///     Returns profile with override substitution based on override case and override properties removed
        /// </summary>
        /// <param name="overrideCase"></param>
        /// <returns></returns>
        public SoftwareStatementProfile ApplyOverrides(string? overrideCase)
        {
            var newObject = new SoftwareStatementProfile
            {
                SoftwareStatement = SoftwareStatement,
                TransportCertificateProfileId = TransportCertificateProfileId,
                SigningCertificateProfileId = SigningCertificateProfileId,
                DefaultFragmentRedirectUrl = DefaultFragmentRedirectUrl
            };

            if (overrideCase is null)
            {
                return newObject;
            }

            if (TransportCertificateProfileIdOverrides.TryGetValue(
                    overrideCase,
                    out string? transportCertificateProfileId))
            {
                newObject.TransportCertificateProfileId = transportCertificateProfileId;
            }

            if (SigningCertificateProfileIdOverrides.TryGetValue(
                    overrideCase,
                    out string? signingCertificateProfileId))
            {
                newObject.SigningCertificateProfileId = signingCertificateProfileId;
            }

            return newObject;
        }
    }

    /// <summary>
    ///     Software statement profiles settings. This is a dictionary of <see cref="SoftwareStatementProfile" /> objects
    ///     keyed by ID to Open Banking Connector. It is expected this is provided by a collection of key secrets.
    /// </summary>
    public class SoftwareStatementProfilesSettings : Dictionary<string, SoftwareStatementProfileWithOverrideProperties>,
        ISettings<SoftwareStatementProfilesSettings>
    {
        public string SettingsGroupName => "OpenBankingConnector:SoftwareStatementProfiles";

        /// <summary>
        ///     Placeholder. Validation is performed only on individual <see cref="SoftwareStatementProfile" /> entries
        ///     that are to be used by Open Banking Connector. This validation is performed in the constructor
        ///     of <see cref="ProcessedSoftwareStatementProfileStore" />
        /// </summary>
        /// <returns></returns>
        public SoftwareStatementProfilesSettings Validate()
        {
            foreach ((string key, SoftwareStatementProfileWithOverrideProperties value) in this)
            {
                if (string.IsNullOrEmpty(value.SoftwareStatement))
                {
                    throw new ArgumentException(
                        "Configuration or key secrets error: " +
                        $"No non-empty SoftwareStatement provided for SoftwareStatementProfile {key}.");
                }

                if (string.IsNullOrEmpty(value.TransportCertificateProfileId))
                {
                    throw new ArgumentException(
                        "Configuration or key secrets error: " +
                        $"No non-empty TransportCertificateProfileId provided for SoftwareStatementProfile {key}.");
                }

                if (string.IsNullOrEmpty(value.SigningCertificateProfileId))
                {
                    throw new ArgumentException(
                        "Configuration or key secrets error: " +
                        $"No non-empty SigningCertificateProfileId provided for SoftwareStatementProfile {key}.");
                }

                if (string.IsNullOrEmpty(value.DefaultFragmentRedirectUrl))
                {
                    throw new ArgumentException(
                        "Configuration or key secrets error: " +
                        $"No non-empty DefaultFragmentRedirectUrl provided for SoftwareStatementProfile {key}.");
                }
            }

            return this;
        }
    }
}
