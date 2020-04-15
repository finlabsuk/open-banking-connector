// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Validation
{
    public class AuthorisationCallbackDataValidatorTests
    {
        [Fact]
        public void Validate_ModeIsNull()
        {
            var validator = new AuthorisationCallbackDataValidator();

            var data = new AuthorisationCallbackData(null, null);

            var results = validator.Validate(data).Errors;

            results.Should().HaveCount(2);
        }

        [Fact]
        public void Validate_BodyIsNull()
        {
            var validator = new AuthorisationCallbackDataValidator();

            var data = new AuthorisationCallbackData("a", null);

            var results = validator.Validate(data).Errors;

            results.Should().HaveCount(1);
        }


        [Fact]
        public void Validate_BodyIsDefault()
        {
            var validator = new AuthorisationCallbackDataValidator();

            var data = new AuthorisationCallbackData("a", new AuthorisationCallbackPayload(null, null));

            var results = validator.Validate(data).Errors;

            results.Should().HaveCountGreaterThan(1);
        }


        [Fact]
        public void Validate_BodyIsValid()
        {
            var validator = new AuthorisationCallbackDataValidator();

            var data = new AuthorisationCallbackData("a",
                new AuthorisationCallbackPayload("a", "a")
                {
                    IdToken = "a",
                    Nonce = null,
                }
            );

            var results = validator.Validate(data).Errors;

            results.Should().HaveCount(0);
        }
    }
}
