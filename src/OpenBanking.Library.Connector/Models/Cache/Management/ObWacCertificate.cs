// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;

public enum SubjectDnOrgIdEncoding
{
    StringAttributeType, // default
    DottedDecimalAttributeType,
    DottedDecimalAttributeTypeWithStringValue
}

public class ObWacCertificate
{
    public ObWacCertificate(
        TransportCertificateProfile transportCertificateProfile,
        string id,
        string? overrideCase,
        HttpClientSettings httpClientSettings,
        IInstrumentationClient instrumentationClient,
        TppReportingMetrics tppReportingMetrics)
    {
        // Validate transport certificate profile
        ValidationResult validationResult2 = new OBTransportCertificateProfileValidator()
            .Validate(transportCertificateProfile);
        validationResult2.ProcessValidationResultsAndRaiseErrors(
            "Configuration/secrets error",
            $"Validation failure when checking transport certificate profile with ID {id}" +
            (overrideCase is null
                ? "."
                : $" and override case {overrideCase}."
            ));

        // Create HttpMessageHandler with transport certificates
        var transportCerts = new List<X509Certificate2>();
        X509Certificate2 transportCert =
            CertificateFactories.CreateCertWithKey(
                transportCertificateProfile.Certificate,
                transportCertificateProfile.AssociatedKey) ??
            throw new ArgumentException(
                $"Encountered problem when processing transport certificate from transport certificate profile with ID {id}" +
                (overrideCase is null
                    ? "."
                    : $" and override case {overrideCase}."
                ));
        transportCerts.Add(transportCert);

        IServerCertificateValidator? serverCertificateValidator = null;
        if (transportCertificateProfile.DisableTlsCertificateVerification)
        {
            serverCertificateValidator = new DefaultServerCertificateValidator();
        }

        TransportCertificate = transportCertificateProfile.Certificate;
        TransportCertificateId = id;

        ApiClient = new ApiClient(
            instrumentationClient,
            httpClientSettings.PooledConnectionLifetimeSeconds,
            tppReportingMetrics,
            transportCerts,
            serverCertificateValidator);

        SubjectDn = new ConcurrentDictionary<SubjectDnOrgIdEncoding, string>
        {
            [SubjectDnOrgIdEncoding.StringAttributeType] =
                GetSubjectDn(transportCert, SubjectDnOrgIdEncoding.StringAttributeType),
            [SubjectDnOrgIdEncoding.DottedDecimalAttributeType] =
                GetSubjectDn(transportCert, SubjectDnOrgIdEncoding.DottedDecimalAttributeType),
            [SubjectDnOrgIdEncoding.DottedDecimalAttributeTypeWithStringValue] = GetSubjectDn(
                transportCert,
                SubjectDnOrgIdEncoding.DottedDecimalAttributeTypeWithStringValue)
        };
    }

    public IApiClient ApiClient { get; }

    public string TransportCertificate { get; } // pass-through for migration

    public string TransportCertificateId { get; } // pass-through for migration

    public ConcurrentDictionary<SubjectDnOrgIdEncoding, string> SubjectDn { get; }

    public static ObWacCertificate CreateInstance(
        ObWacCertificateEntity obWac,
        string associatedKey,
        ISecretProvider secretProvider,
        HttpClientSettings httpClientSettings,
        IInstrumentationClient instrumentationClient,
        TppReportingMetrics tppReportingMetrics) =>
        new(
            new TransportCertificateProfile
            {
                Active = true,
                DisableTlsCertificateVerification = false,
                Certificate = obWac.Certificate,
                AssociatedKey = associatedKey
            },
            obWac.Id.ToString(),
            null,
            httpClientSettings,
            instrumentationClient,
            tppReportingMetrics);

    private static string GetSubjectDn(X509Certificate2 transportCert, SubjectDnOrgIdEncoding subjectDnOrgIdEncoding)
    {
        // Get subject DN
        X500DistinguishedName subjectDnObject = transportCert.SubjectName;

        // Get subject DN string removing unwanted space in delimiter
        string subjectDn = subjectDnObject.Name.Replace(", ", ",");

        // Update organizationIdentifier attribute type
        if (subjectDnOrgIdEncoding is not SubjectDnOrgIdEncoding.StringAttributeType)
        {
            subjectDn = subjectDn.Replace("OID.2.5.4.97", "2.5.4.97"); // fix currently necessary on Windows
            subjectDn = subjectDn.Replace("organizationIdentifier", "2.5.4.97");
        }
        else
        {
            subjectDn = subjectDn.Replace(
                "OID.2.5.4.97",
                "organizationIdentifier"); // fix currently necessary on Windows
        }

        // Update organizationIdentifier attribute value if required
        if (subjectDnOrgIdEncoding is SubjectDnOrgIdEncoding.DottedDecimalAttributeType)
        {
            // Get OID 2.5.4.97 value
            X500RelativeDistinguishedName orgIdObject =
                subjectDnObject
                    .EnumerateRelativeDistinguishedNames()
                    .Single(
                        x =>
                            x.HasMultipleElements is false &&
                            x.GetSingleElementType() is { Value: "2.5.4.97" });
            string orgId = orgIdObject.GetSingleElementValue() ?? throw new InvalidOperationException();

            // BER-encode value
            var writer = new AsnWriter(AsnEncodingRules.BER);
            writer.WriteCharacterString(UniversalTagNumber.PrintableString, orgId);
            byte[] output = writer.Encode();
            string newOrgId = "#" + Convert.ToHexString(output);

            // Update attribute value
            subjectDn = subjectDn.Replace(orgId, newOrgId);
        }

        return subjectDn;
    }

    public static string GetCacheKey(Guid obWacId) => string.Join(":", "ob_wac_certificate", obWacId);
}
