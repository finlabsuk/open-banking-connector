// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;

public class ProcessedTransportCertificateProfile
{
    public ProcessedTransportCertificateProfile(
        TransportCertificateProfile transportCertificateProfile,
        string id,
        string? overrideCase,
        IInstrumentationClient instrumentationClient)
    {
        // Log processing message
        string message =
            "Configuration/secrets info: " +
            $"Processing Transport Certificate Profile with ID {id}";
        message += overrideCase is null
            ? "."
            : $" and override {overrideCase}.";
        instrumentationClient.Info(message);

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

        ApiClient = new ApiClient(instrumentationClient, transportCerts, serverCertificateValidator);

        CertificateType = transportCertificateProfile.CertificateType;

        SubjectDnWithDottedDecimalOrgIdAttribute = GetSubjectDn(transportCert, true);

        SubjectDn = GetSubjectDn(transportCert, false);
    }

    public IApiClient ApiClient { get; }

    public TransportCertificateType CertificateType { get; }

    public string SubjectDnWithDottedDecimalOrgIdAttribute { get; }

    public string SubjectDn { get; }

    private string GetSubjectDn(X509Certificate2 transportCert, bool useDottedDecimalOrgIdAttribute)
    {
        // Get subject DN
        X500DistinguishedName subjectDnObject = transportCert.SubjectName;

        // Get subject DN string removing unwanted space in delimiter
        string subjectDn = subjectDnObject.Name.Replace(", ", ",");

        // Update organizationIdentifier attribute type
        if (useDottedDecimalOrgIdAttribute)
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
        if (useDottedDecimalOrgIdAttribute)
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
}
