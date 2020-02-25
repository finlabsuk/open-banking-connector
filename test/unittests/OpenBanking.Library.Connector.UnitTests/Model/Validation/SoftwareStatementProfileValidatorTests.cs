// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries;
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
                    ObSigningKey = "a",
                    ObSigningKid = "a",
                    ObSigningPem = "a",
                    ObTransportKey = "a",
                    ObTransportPem = "a",
                    SoftwareStatement = $"{value}.{value}.{value}"
                };

                var results = new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value) && !value.Contains('.'));
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property Validate_ObSigningKey(string value)
        {
            Func<bool> rule = () =>
            {
                var profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    ObSigningKey = value,
                    ObSigningKid = "a",
                    ObSigningPem = "a",
                    ObTransportKey = "a",
                    ObTransportPem = "a",
                    SoftwareStatement = "a.b.c"
                };

                var results = new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value));
        }


        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property Validate_ObSigningKid(string value)
        {
            Func<bool> rule = () =>
            {
                var profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    ObSigningKey = "a",
                    ObSigningKid = value,
                    ObSigningPem = "a",
                    ObTransportKey = "a",
                    ObTransportPem = "a",
                    SoftwareStatement = "a.b.c"
                };

                var results = new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value));
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property Validate_ObSigningPem(string value)
        {
            Func<bool> rule = () =>
            {
                var profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    ObSigningKey = "a",
                    ObSigningKid = "a",
                    ObSigningPem = value,
                    ObTransportKey = "a",
                    ObTransportPem = "a",
                    SoftwareStatement = "a.b.c"
                };

                var results = new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value));
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property Validate_ObTransportKey(string value)
        {
            Func<bool> rule = () =>
            {
                var profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    ObSigningKey = "a",
                    ObSigningKid = "a",
                    ObSigningPem = "a",
                    ObTransportKey = value,
                    ObTransportPem = "a",
                    SoftwareStatement = "a.b.c"
                };

                var results = new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value));
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property Validate_ObTransportPem(string value)
        {
            Func<bool> rule = () =>
            {
                var profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = "http://test.com",
                    ObSigningKey = "a",
                    ObSigningKid = "a",
                    ObSigningPem = "a",
                    ObTransportKey = "a",
                    ObTransportPem = value,
                    SoftwareStatement = "a.b.c"
                };

                var results = new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 0;
            };

            return rule.When(!string.IsNullOrWhiteSpace(value));
        }

        [Property(Arbitrary = new[] { typeof(BaseUrlArbitrary) })]
        public Property Validate_DefaultFragmentRedirectUrl(Uri value)
        {
            Func<bool> rule = () =>
            {
                var profile = new SoftwareStatementProfile
                {
                    DefaultFragmentRedirectUrl = value.ToString(),
                    ObSigningKey = "a",
                    ObSigningKid = "a",
                    ObSigningPem = "a",
                    ObTransportKey = "a",
                    ObTransportPem = "a",
                    SoftwareStatement = "a.b.c"
                };

                var results = new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

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
                    ObSigningKey = "a",
                    ObSigningKid = "a",
                    ObSigningPem = "a",
                    ObTransportKey = "a",
                    ObTransportPem = "a",
                    SoftwareStatement = "a.b.c"
                };

                var results = new SoftwareStatementProfileValidator().Validate(profile).Errors.ToList();

                return results.Count == 1;
            };

            return rule.ToProperty();
        }
    }
}
