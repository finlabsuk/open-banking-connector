// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;

public class OBSealKey
{
    public OBSealKey(string keyId, string key)
    {
        KeyId = keyId;
        Key = key;
    }

    // Open Banking Signing Key ID as string, e.g. "ABC"
    public string KeyId { get; }

    // Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
    public string Key { get; }
}

/// <summary>
///     Processed software statement profile generated at start-up which includes
///     information from a <see cref="SoftwareStatementProfile" />, a <see cref="TransportCertificateProfile" />, and a
///     <see cref="SigningCertificateProfile" />
/// </summary>
public class ProcessedSoftwareStatementProfile
{
    private readonly string? _defaultFragmentRedirectUrl;
    private readonly string? _defaultQueryRedirectUrl;

    public ProcessedSoftwareStatementProfile(
        ProcessedTransportCertificateProfile processedTransportCertificateProfile,
        ProcessedSigningCertificateProfile processedSigningCertificateProfile,
        SoftwareStatementProfile softwareStatementProfile,
        string id,
        string? overrideCase,
        IInstrumentationClient instrumentationClient)
    {
        // Log processing message
        string message =
            "Configuration/secrets info: " +
            $"Processing Software Statement Profile with ID {id}";
        message += overrideCase is null
            ? "."
            : $" and override {overrideCase}.";
        instrumentationClient.Info(message);

        // Validate software statement profile
        ValidationResult validationResult = new SoftwareStatementProfileValidator()
            .Validate(softwareStatementProfile);
        validationResult.ProcessValidationResultsAndRaiseErrors(
            "Configuration/secrets error",
            $"Validation failure when checking software statement profile with ID {id}" +
            (overrideCase is null
                ? "."
                : $" and override case {overrideCase}."
            ));

        // Pass-through properties
        OBSealKey = new OBSealKey(
            processedSigningCertificateProfile.AssociatedKeyId,
            processedSigningCertificateProfile.AssociatedKey);
        TransportCertificateSubjectDn = processedTransportCertificateProfile
            .SubjectDn;
        Id = id;
        OverrideCase = overrideCase;
        ApiClient = processedTransportCertificateProfile.ApiClient;
        OrganisationId = softwareStatementProfile.OrganisationId;
        SoftwareId = softwareStatementProfile.SoftwareId;
        SandboxEnvironment = softwareStatementProfile.SandboxEnvironment;

        TransportCertificate = processedTransportCertificateProfile.TransportCertificate;
        TransportCertificateId = processedTransportCertificateProfile.TransportCertificateId;
        SigningCertificate = processedSigningCertificateProfile.SigningCertificate;
        SigningCertificateId = processedSigningCertificateProfile.SigningCertificateId;

        if (!string.IsNullOrEmpty(softwareStatementProfile.DefaultQueryRedirectUrl))
        {
            _defaultQueryRedirectUrl = softwareStatementProfile.DefaultQueryRedirectUrl;
        }

        if (!string.IsNullOrEmpty(softwareStatementProfile.DefaultFragmentRedirectUrl))
        {
            _defaultFragmentRedirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
        }
    }

    public OBSealKey OBSealKey { get; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = query.
    /// </summary>
    public string DefaultQueryRedirectUrl
    {
        get
        {
            if (_defaultQueryRedirectUrl is null)
            {
                throw new ArgumentException(
                    $"No non-empty DefaultQueryRedirectUrl provided in software statement profile with ID {Id}");
            }

            return _defaultQueryRedirectUrl;
        }
    }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = fragment.
    /// </summary>
    public string DefaultFragmentRedirectUrl
    {
        get
        {
            if (_defaultFragmentRedirectUrl is null)
            {
                throw new ArgumentException(
                    $"No non-empty DefaultFragmentRedirectUrl provided in software statement profile with ID {Id}");
            }

            return _defaultFragmentRedirectUrl;
        }
    }

    public ConcurrentDictionary<SubjectDnOrgIdEncoding, string> TransportCertificateSubjectDn { get; }

    public IApiClient ApiClient { get; }

    public string Id { get; }

    public string SigningCertificate { get; } // pass-through for migration

    public string SigningCertificateId { get; } // pass-through for migration

    public string TransportCertificate { get; } // pass-through for migration

    public string TransportCertificateId { get; } // pass-through for migration

    public string? OverrideCase { get; }

    public string OrganisationId { get; }

    public string SoftwareId { get; }

    public bool SandboxEnvironment { get; }

    public string GetRedirectUri(
        OAuth2ResponseMode responseMode,
        string? registrationFragmentRedirectUrl,
        string? registrationQueryRedirectUrl) =>
        responseMode switch
        {
            OAuth2ResponseMode.Query => registrationQueryRedirectUrl ?? DefaultQueryRedirectUrl,
            OAuth2ResponseMode.Fragment => registrationFragmentRedirectUrl ?? DefaultFragmentRedirectUrl,
            //OAuth2ResponseMode.FormPost => expr,
            _ => throw new ArgumentOutOfRangeException(nameof(responseMode), responseMode, null)
        };
}
