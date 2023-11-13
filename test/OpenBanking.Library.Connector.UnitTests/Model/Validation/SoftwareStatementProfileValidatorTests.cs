// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries;
using FluentValidation.Results;
using FsCheck;
using FsCheck.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Validation;

public class SoftwareStatementProfileValidatorTests
{
    [Property(Verbose = PropertyTests.VerboseTests)]
    public Property Validate_SoftwareStatement(string value)
    {
        Func<bool> rule = () =>
        {
            var profile = new SoftwareStatementProfile { DefaultFragmentRedirectUrl = "http://test.com" };

            List<ValidationFailure> results =
                new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

            return results.Count == 0;
        };

        return rule.When(!string.IsNullOrWhiteSpace(value) && !value.Contains('.'));
    }

    [Property(Verbose = PropertyTests.VerboseTests)]
    public Property Validate_ObSigningKey(string value)
    {
        Func<bool> rule = () =>
        {
            var profile = new SigningCertificateProfile
            {
                AssociatedKey = value,
                AssociatedKeyId = "a"
            };

            List<ValidationFailure> results =
                new OBSigningCertificateProfileValidator().Validate(profile).Errors.ToList();

            return results.Count == 0;
        };

        return rule.When(!string.IsNullOrWhiteSpace(value));
    }


    [Property(Verbose = PropertyTests.VerboseTests)]
    public Property Validate_ObSigningKid(string value)
    {
        Func<bool> rule = () =>
        {
            var profile = new SigningCertificateProfile
            {
                AssociatedKey = "a",
                AssociatedKeyId = value
            };

            List<ValidationFailure> results =
                new OBSigningCertificateProfileValidator().Validate(profile).Errors.ToList();

            return results.Count == 0;
        };

        return rule.When(!string.IsNullOrWhiteSpace(value));
    }

    [Property(Verbose = PropertyTests.VerboseTests)]
    public Property Validate_ObSigningPem(string value)
    {
        Func<bool> rule = () =>
        {
            var profile = new SigningCertificateProfile
            {
                AssociatedKey = "a",
                AssociatedKeyId = "a"
            };

            List<ValidationFailure> results =
                new OBSigningCertificateProfileValidator().Validate(profile).Errors.ToList();

            return results.Count == 0;
        };

        return rule.When(!string.IsNullOrWhiteSpace(value));
    }

    [Property(Verbose = PropertyTests.VerboseTests)]
    public Property Validate_ObTransportKey(string value)
    {
        Func<bool> rule = () =>
        {
            var profile = new TransportCertificateProfile
            {
                AssociatedKey = value,
                Certificate = "a"
            };

            List<ValidationFailure> results =
                new OBTransportCertificateProfileValidator().Validate(profile).Errors.ToList();

            return results.Count == 0;
        };

        return rule.When(!string.IsNullOrWhiteSpace(value));
    }

    [Property(Verbose = PropertyTests.VerboseTests)]
    public Property Validate_ObTransportPem(string value)
    {
        Func<bool> rule = () =>
        {
            var profile = new TransportCertificateProfile
            {
                AssociatedKey = "a",
                Certificate = value
            };

            List<ValidationFailure> results =
                new OBTransportCertificateProfileValidator().Validate(profile).Errors.ToList();

            return results.Count == 0;
        };

        return rule.When(!string.IsNullOrWhiteSpace(value));
    }

    [Property(Arbitrary = new[] { typeof(BaseUrlArbitrary) })]
    public Property Validate_DefaultFragmentRedirectUrl(Uri value)
    {
        Func<bool> rule = () =>
        {
            var profile = new TransportCertificateProfile
            {
                AssociatedKey = "a",
                Certificate = "a"
            };

            List<ValidationFailure> results =
                new OBTransportCertificateProfileValidator().Validate(profile).Errors.ToList();

            return results.Count == 0;
        };

        return rule.ToProperty();
    }


    [Property(Arbitrary = new[] { typeof(InvalidUriArbitrary) })]
    public Property Validate_DefaultFragmentRedirectUrl_InvalidString(string value)
    {
        Func<bool> rule = () =>
        {
            var profile = new SoftwareStatementProfile { DefaultFragmentRedirectUrl = value };

            List<ValidationFailure> results =
                new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

            return results.Count == 1;
        };

        return rule.ToProperty();
    }
}
