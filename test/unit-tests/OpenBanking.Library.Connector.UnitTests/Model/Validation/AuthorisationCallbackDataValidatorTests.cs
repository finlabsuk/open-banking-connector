// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
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
            AuthorisationCallbackDataValidator? validator = new AuthorisationCallbackDataValidator();

            AuthorisationCallbackData? data = new AuthorisationCallbackData(responseMode: null, response: null);

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCount(2);
        }

        [Fact]
        public void Validate_BodyIsNull()
        {
            AuthorisationCallbackDataValidator? validator = new AuthorisationCallbackDataValidator();

            AuthorisationCallbackData? data = new AuthorisationCallbackData(responseMode: "a", response: null);

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCount(1);
        }


        [Fact]
        public void Validate_BodyIsDefault()
        {
            AuthorisationCallbackDataValidator? validator = new AuthorisationCallbackDataValidator();

            AuthorisationCallbackData? data = new AuthorisationCallbackData(
                responseMode: "a",
                response: new AuthorisationCallbackPayload(authorisationCode: null, state: null));

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCountGreaterThan(1);
        }


        [Fact]
        public void Validate_BodyIsValid()
        {
            AuthorisationCallbackDataValidator? validator = new AuthorisationCallbackDataValidator();

            AuthorisationCallbackData? data = new AuthorisationCallbackData(
                responseMode: "a",
                response: new AuthorisationCallbackPayload(authorisationCode: "a", state: "a")
                {
                    Id_Token = "a",
                    Nonce = null,
                });

            IList<ValidationFailure>? results = validator.Validate(data).Errors;

            results.Should().HaveCount(0);
        }
    }
}
