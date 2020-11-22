// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
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
            AuthorisationRedirectObjectValidator? validator = new AuthorisationRedirectObjectValidator();

            AuthorisationRedirectObject? data = new AuthorisationRedirectObject(responseMode: null, response: null);

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCount(2);
        }

        [Fact]
        public void Validate_BodyIsNull()
        {
            AuthorisationRedirectObjectValidator? validator = new AuthorisationRedirectObjectValidator();

            AuthorisationRedirectObject? data = new AuthorisationRedirectObject(responseMode: "a", response: null);

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCount(1);
        }


        [Fact]
        public void Validate_BodyIsDefault()
        {
            AuthorisationRedirectObjectValidator? validator = new AuthorisationRedirectObjectValidator();

            AuthorisationRedirectObject? data = new AuthorisationRedirectObject(
                responseMode: "a",
                response: new AuthorisationCallbackPayload());

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCountGreaterThan(1);
        }


        [Fact]
        public void Validate_BodyIsValid()
        {
            AuthorisationRedirectObjectValidator? validator = new AuthorisationRedirectObjectValidator();

            AuthorisationRedirectObject? data = new AuthorisationRedirectObject(
                responseMode: "a",
                response: new AuthorisationCallbackPayload
                {
                    Code = "a",
                    State = "a",
                    Id_Token = "a",
                    Nonce = null,
                });

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCount(0);
        }
    }
}
