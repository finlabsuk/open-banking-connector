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

    public static async Task<ObSealCertificate> CreateInstance(
        ObSealCertificateEntity obSeal,
        ISecretProvider secretProvider,
        IInstrumentationClient instrumentationClient) =>
        new(
            new SigningCertificateProfile
            {
                Active = true,
                AssociatedKey = await secretProvider.GetSecretAsync(obSeal.AssociatedKey),
                AssociatedKeyId = obSeal.AssociatedKeyId,
                Certificate = obSeal.Certificate
            },
            obSeal.Id.ToString(),
            instrumentationClient);

    public static string GetCacheKey(Guid obSealId) => string.Join(":", "ob_seal_certificate", obSealId);
}
