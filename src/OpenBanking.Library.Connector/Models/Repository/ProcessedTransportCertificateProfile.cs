// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

        CertificateDnWithHexDottedDecimalAttributeValues =
            transportCertificateProfile.CertificateDnWithHexDottedDecimalAttributeValues;

        CertificateDnWithStringDottedDecimalAttributeValues =
            transportCertificateProfile.CertificateDnWithStringDottedDecimalAttributeValues;
    }

    public IApiClient ApiClient { get; }

    public TransportCertificateType CertificateType { get; }

    public string CertificateDnWithHexDottedDecimalAttributeValues { get; set; }

    public string CertificateDnWithStringDottedDecimalAttributeValues { get; set; }
}
