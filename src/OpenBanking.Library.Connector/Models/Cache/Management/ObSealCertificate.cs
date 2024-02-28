// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;

public class ObSealCertificate
{
    public ObSealCertificate(
        SigningCertificateProfile signingCertificateProfile,
        string id,
        IInstrumentationClient instrumentationClient)
    {
        // Validate signing certificate profile
        ValidationResult validationResult3 = new OBSigningCertificateProfileValidator()
            .Validate(signingCertificateProfile);
        validationResult3.ProcessValidationResultsAndRaiseErrors(
            "Configuration/secrets error",
            $"Validation failure when checking signing certificate profile with ID {id}.");

        AssociatedKey = signingCertificateProfile.AssociatedKey;
        AssociatedKeyId = signingCertificateProfile.AssociatedKeyId;

        SigningCertificate = signingCertificateProfile.Certificate;
        SigningCertificateId = id;
    }

    public string AssociatedKeyId { get; }

    public string AssociatedKey { get; }

    public string SigningCertificate { get; } // pass-through for migration

    public string SigningCertificateId { get; } // pass-through for migration

    public OBSealKey ObSealKey => new(AssociatedKeyId, AssociatedKey);

    public static ObSealCertificate GetProcessedObSeal(
        ISecretProvider secretProvider,
        IInstrumentationClient instrumentationClient,
        ObSealCertificateEntity obSeal)
    {
        if (!secretProvider.TryGetSecret(obSeal.AssociatedKey.Name, out string? associatedKey))
        {
            string message =
                $"OBSeal signing certificate with ID {obSeal.Id} " +
                $"specifies AssociatedKey with name {obSeal.AssociatedKey.Name} " +
                "but no such value can be found. Any software statement(s) depending " +
                "on this OBSeal signing certificate will not be able to be used.";
            throw new KeyNotFoundException(message);
        }
        var processedSigningCertificateProfile = new ObSealCertificate(
            new SigningCertificateProfile
            {
                Active = true,
                AssociatedKey = associatedKey,
                AssociatedKeyId = obSeal.AssociatedKeyId,
                Certificate = obSeal.Certificate
            },
            obSeal.Id.ToString(),
            instrumentationClient);
        return processedSigningCertificateProfile;
    }

    public static string GetCacheKey(Guid obSealId) => string.Join(":", "ob_seal_certificate", obSealId);
}
