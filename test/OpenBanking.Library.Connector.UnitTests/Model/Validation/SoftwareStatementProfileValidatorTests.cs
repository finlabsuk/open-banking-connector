﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
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
                SoftwareStatementProfile profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    SigningKey = "a",
                    SigningKeyId = "a",
                    SigningCertificate = "a",
                    TransportKey = "a",
                    TransportCertificate = "a",
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
                SoftwareStatementProfile profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    SigningKey = value,
                    SigningKeyId = "a",
                    SigningCertificate = "a",
                    TransportKey = "a",
                    TransportCertificate = "a",
                    SoftwareStatement = "a.b.c"
                };

                List<ValidationFailure> results =
                    new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value));
        }


        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property Validate_ObSigningKid(string value)
        {
            Func<bool> rule = () =>
            {
                SoftwareStatementProfile profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    SigningKey = "a",
                    SigningKeyId = value,
                    SigningCertificate = "a",
                    TransportKey = "a",
                    TransportCertificate = "a",
                    SoftwareStatement = "a.b.c"
                };

                List<ValidationFailure> results =
                    new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value));
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property Validate_ObSigningPem(string value)
        {
            Func<bool> rule = () =>
            {
                SoftwareStatementProfile profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    SigningKey = "a",
                    SigningKeyId = "a",
                    SigningCertificate = value,
                    TransportKey = "a",
                    TransportCertificate = "a",
                    SoftwareStatement = "a.b.c"
                };

                List<ValidationFailure> results =
                    new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value));
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property Validate_ObTransportKey(string value)
        {
            Func<bool> rule = () =>
            {
                SoftwareStatementProfile profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    SigningKey = "a",
                    SigningKeyId = "a",
                    SigningCertificate = "a",
                    TransportKey = value,
                    TransportCertificate = "a",
                    SoftwareStatement = "a.b.c"
                };

                List<ValidationFailure> results =
                    new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value));
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property Validate_ObTransportPem(string value)
        {
            Func<bool> rule = () =>
            {
                SoftwareStatementProfile profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    SigningKey = "a",
                    SigningKeyId = "a",
                    SigningCertificate = "a",
                    TransportKey = "a",
                    TransportCertificate = value,
                    SoftwareStatement = "a.b.c"
                };

                List<ValidationFailure> results =
                    new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value));
        }

        [Property(Arbitrary = new[] { typeof(BaseUrlArbitrary) })]
        public Property Validate_DefaultFragmentRedirectUrl(Uri value)
        {
            Func<bool> rule = () =>
            {
                SoftwareStatementProfile profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = value.ToString(),
                    SigningKey = "a",
                    SigningKeyId = "a",
                    SigningCertificate = "a",
                    TransportKey = "a",
                    TransportCertificate = "a",
                    SoftwareStatement = "a.b.c"
                };

                List<ValidationFailure> results =
                    new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.ToProperty();
        }


        [Property(Arbitrary = new[] { typeof(InvalidUriArbitrary) })]
        public Property Validate_DefaultFragmentRedirectUrl_InvalidString(string value)
        {
            Func<bool> rule = () =>
            {
                SoftwareStatementProfile profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = value,
                    SigningKey = "a",
                    SigningKeyId = "a",
                    SigningCertificate = "a",
                    TransportKey = "a",
                    TransportCertificate = "a",
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
