// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Validation
{
    public class AuthorisationCallbackDataValidatorTests
    {
        [Fact]
        public void Validate_ModeIsNull()
        {
            var validator = new AuthorisationRedirectObjectValidator();

            var data = new AuthorisationRedirectObject(null!, null!);

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCount(2);
        }

        [Fact]
        public void Validate_BodyIsNull()
        {
            var validator = new AuthorisationRedirectObjectValidator();

            var data = new AuthorisationRedirectObject("a", null!);

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCount(1);
        }


        [Fact]
        public void Validate_BodyIsDefault()
        {
            var validator = new AuthorisationRedirectObjectValidator();

            var data = new AuthorisationRedirectObject(
                "a",
                new AuthorisationCallbackPayload("", "", "", null));

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCountGreaterThan(1);
        }


        [Fact]
        public void Validate_BodyIsValid()
        {
            var validator = new AuthorisationRedirectObjectValidator();

            var data = new AuthorisationRedirectObject(
                "a",
                new AuthorisationCallbackPayload("a", "a", "a", null));

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCount(0);
        }
    }
}
