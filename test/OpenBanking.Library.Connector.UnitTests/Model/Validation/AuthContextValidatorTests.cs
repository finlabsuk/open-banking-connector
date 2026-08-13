// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Validators;
using FluentValidation.Results;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Validation;

public class AuthContextValidatorTests
{
    [Fact]
    public void AccountAccessConsentAuthContext_ValidatesRedirectUri()
    {
        var validator = new AccountAccessConsentAuthContextValidator();

        var validCtx = new AccountAccessConsentAuthContext
        {
            AccountAccessConsentId = Guid.NewGuid(),
            RedirectUri = "https://example.com/callback"
        };
        IList<ValidationFailure> validResults = validator.Validate(validCtx).Errors;
        Assert.Empty(validResults);

        var invalidCtx = new AccountAccessConsentAuthContext
        {
            AccountAccessConsentId = Guid.NewGuid(),
            RedirectUri = "not-a-valid-url"
        };
        IList<ValidationFailure> invalidResults = validator.Validate(invalidCtx).Errors;
        Assert.Single(invalidResults);
    }

    [Fact]
    public void DomesticPaymentConsentAuthContext_ValidatesRedirectUri()
    {
        var validator = new DomesticPaymentConsentAuthContextValidator();

        var validCtx = new DomesticPaymentConsentAuthContext
        {
            DomesticPaymentConsentId = Guid.NewGuid(),
            RedirectUri = "https://example.com/callback"
        };
        IList<ValidationFailure> validResults = validator.Validate(validCtx).Errors;
        Assert.Empty(validResults);

        var invalidCtx = new DomesticPaymentConsentAuthContext
        {
            DomesticPaymentConsentId = Guid.NewGuid(),
            RedirectUri = "not-a-valid-url"
        };
        IList<ValidationFailure> invalidResults = validator.Validate(invalidCtx).Errors;
        Assert.Single(invalidResults);
    }

    [Fact]
    public void DomesticVrpConsentAuthContext_ValidatesRedirectUri()
    {
        var validator = new DomesticVrpConsentAuthContextValidator();

        var validCtx = new DomesticVrpConsentAuthContext
        {
            DomesticVrpConsentId = Guid.NewGuid(),
            RedirectUri = "https://example.com/callback"
        };
        IList<ValidationFailure> validResults = validator.Validate(validCtx).Errors;
        Assert.Empty(validResults);

        var invalidCtx = new DomesticVrpConsentAuthContext
        {
            DomesticVrpConsentId = Guid.NewGuid(),
            RedirectUri = "not-a-valid-url"
        };
        IList<ValidationFailure> invalidResults = validator.Validate(invalidCtx).Errors;
        Assert.Single(invalidResults);
    }
}
