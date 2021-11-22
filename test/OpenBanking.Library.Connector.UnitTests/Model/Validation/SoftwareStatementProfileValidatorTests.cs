// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries;
using FluentValidation.Results;
using FsCheck;
using FsCheck.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Validation
{
    public class SoftwareStatementProfileValidatorTests
    {
        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property Validate_SoftwareStatement(string value)
        {
            Func<bool> rule = () =>
            {
                var profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    SoftwareStatement = $"{value}.{value}.{value}"
                };

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
                var profile = new OBSigningCertificateProfile
                {
                    SigningKey = value,
                    SigningKeyId = "a",
                    SigningCertificate = "a",
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
                var profile = new OBSigningCertificateProfile
                {
                    CertificateType = "LegacyOB",
                    SigningKey = "a",
                    SigningKeyId = value,
                    SigningCertificate = "a",
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
                var profile = new OBSigningCertificateProfile
                {
                    SigningKey = "a",
                    SigningKeyId = "a",
                    SigningCertificate = value,
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
                var profile = new OBTransportCertificateProfile
                {
                    TransportKey = value,
                    TransportCertificate = "a",
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
                var profile = new OBTransportCertificateProfile
                {
                    TransportKey = "a",
                    TransportCertificate = value,
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
                var profile = new OBTransportCertificateProfile
                {
                    TransportKey = "a",
                    TransportCertificate = "a",
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
                var profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = value,
                    SoftwareStatement = "a.b.c"
                };

                List<ValidationFailure> results =
                    new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 1;
            };

            return rule.ToProperty();
        }
    }
}
