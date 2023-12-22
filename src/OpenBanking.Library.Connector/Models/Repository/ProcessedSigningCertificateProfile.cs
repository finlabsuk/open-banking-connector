﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;

public class ProcessedSigningCertificateProfile
{
    public ProcessedSigningCertificateProfile(
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
}
