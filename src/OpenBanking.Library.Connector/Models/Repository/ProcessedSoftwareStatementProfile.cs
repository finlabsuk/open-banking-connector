// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;

/// Payload of Open Banking Software Statement type. Fields can be added as required
public class SoftwareStatementPayload
{
    private static readonly Dictionary<string, RegistrationScopeEnum> SoftwareRoleToApiType =
        new()
        {
            ["AISP"] = RegistrationScopeEnum.AccountAndTransaction,
            ["PISP"] = RegistrationScopeEnum.PaymentInitiation,
            ["CBPII"] = RegistrationScopeEnum.FundsConfirmation
        };

    [JsonProperty("software_on_behalf_of_org")]
    public string SoftwareOnBehalfOfOrg = null!;

    [JsonProperty("software_id")]
    public string SoftwareId { get; set; } = null!;

    [JsonProperty("software_client_id")]
    public string SoftwareClientId { get; set; } = null!;

    [JsonProperty("software_client_name")]
    public string SoftwareClientName { get; set; } = null!;

    [JsonProperty("software_client_description")]
    public string SoftwareClientDescription { get; set; } = null!;

    [JsonProperty("software_version")]
    public float SoftwareVersion { get; set; }

    [JsonProperty("software_client_uri")]
    public string SoftwareClientUri { get; set; } = null!;

    [JsonProperty("software_redirect_uris")]
    public List<string> SoftwareRedirectUris { get; set; } = null!;

    [JsonProperty("software_roles")]
    public string[] SoftwareRoles { get; set; } = null!;

    [JsonProperty("org_id")]
    public string OrgId { get; set; } = null!;

    [JsonProperty("org_name")]
    public string OrgName { get; set; } = null!;

    public RegistrationScopeEnum RegistrationScope =>
        SoftwareRoles.Select(role => SoftwareRoleToApiType[role]).Aggregate(
            RegistrationScopeEnum.None,
            (current, next) => current | next);
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
        SigningKeyId = processedSigningCertificateProfile.AssociatedKeyId;
        SigningKey = processedSigningCertificateProfile.AssociatedKey;
        TransportCertificateType = processedTransportCertificateProfile.CertificateType;
        TransportCertificateDnWithHexDottedDecimalAttributeValues =
            processedTransportCertificateProfile.CertificateDnWithHexDottedDecimalAttributeValues;
        TransportCertificateDnWithStringDottedDecimalAttributeValues = processedTransportCertificateProfile
            .CertificateDnWithStringDottedDecimalAttributeValues;
        Id = id;
        OverrideCase = overrideCase;

        // Break software statement into components
        string[] softwareStatementComponentsBase64 =
            softwareStatementProfile.SoftwareStatement.Split(new[] { '.' });
        if (softwareStatementComponentsBase64.Length != 3)
        {
            throw new ArgumentException("softwareStatementComponentsBase64 needs 3 components.");
        }

        SoftwareStatementHeaderBase64 = softwareStatementComponentsBase64[0];
        SoftwareStatementPayloadBase64 = softwareStatementComponentsBase64[1];
        SoftwareStatementPayload =
            SoftwareStatementPayloadFromBase64(softwareStatementComponentsBase64[1]);
        SoftwwareStatementSignatureBase64 = softwareStatementComponentsBase64[2];

        if (SoftwareStatement != softwareStatementProfile.SoftwareStatement)
        {
            throw new InvalidOperationException("Can't correctly process software statement");
        }

        if (!string.IsNullOrEmpty(softwareStatementProfile.DefaultQueryRedirectUrl))
        {
            if (!SoftwareStatementPayload.SoftwareRedirectUris.Contains(
                    softwareStatementProfile.DefaultQueryRedirectUrl))
            {
                throw new ArgumentException(
                    $"Software statement profile with ID {id} contains DefaultQueryRedirectUrl {softwareStatementProfile.DefaultQueryRedirectUrl} " +
                    "which is not included in software statement software_redirect_uris field.");
            }

            _defaultQueryRedirectUrl = softwareStatementProfile.DefaultQueryRedirectUrl;
        }

        if (!string.IsNullOrEmpty(softwareStatementProfile.DefaultFragmentRedirectUrl))
        {
            if (!SoftwareStatementPayload.SoftwareRedirectUris.Contains(
                    softwareStatementProfile.DefaultFragmentRedirectUrl))
            {
                throw new ArgumentException(
                    $"Software statement profile with ID {id} contains DefaultFragmentRedirectUrl {softwareStatementProfile.DefaultFragmentRedirectUrl} " +
                    "which is not included in software statement software_redirect_uris field.");
            }

            _defaultFragmentRedirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
        }

        // Api client
        ApiClient = processedTransportCertificateProfile.ApiClient;
    }

    public string SoftwareStatementHeaderBase64 { get; }

    // TODO: Remove this once SoftwareStatementPayload stores all fields, this duplicates info in SoftwareStatementPayload
    public string SoftwareStatementPayloadBase64 { get; }

    public SoftwareStatementPayload SoftwareStatementPayload { get; }

    public string SoftwwareStatementSignatureBase64 { get; }

    /// Software statement as string, e.g. "A.B.C"
    public string SoftwareStatement =>
        new[]
        {
            SoftwareStatementHeaderBase64,
            SoftwareStatementPayloadBase64,
            SoftwwareStatementSignatureBase64
        }.JoinString(".");

    /// Open Banking Signing Key ID as string, e.g. "ABC"
    public string SigningKeyId { get; }

    /// Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
    public string SigningKey { get; }

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

    public TransportCertificateType TransportCertificateType { get; }

    public string TransportCertificateDnWithHexDottedDecimalAttributeValues { get; }

    public string TransportCertificateDnWithStringDottedDecimalAttributeValues { get; }

    public IApiClient ApiClient { get; }

    public string Id { get; }

    public string? OverrideCase { get; }

    public string SoftwareStatementPayloadToBase64(SoftwareStatementPayload payload)
    {
        string jsonData = JsonConvert.SerializeObject(payload);
        return Base64UrlEncoder.Encode(jsonData);
    }

    public SoftwareStatementPayload SoftwareStatementPayloadFromBase64(string payloadBase64)
    {
        // Perform conversion
        string payloadString = Base64UrlEncoder.Decode(payloadBase64);
        SoftwareStatementPayload newObject =
            JsonConvert.DeserializeObject<SoftwareStatementPayload>(payloadString) ??
            throw new ArgumentException("Cannot de-serialise software statement");

        // Check reverse conversion works or throw
        // (If reverse conversion fails, we can never re-generate base64 correctly)
        // if (payloadBase64 != SoftwareStatementPayloadToBase64(newObject))
        // {
        //     throw new ArgumentException("Please update SoftwareStatementPayload type to support your software statement");
        // }

        return newObject;
    }
}
