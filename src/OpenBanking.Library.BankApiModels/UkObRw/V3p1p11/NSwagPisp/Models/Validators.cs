// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.NSwagPisp.Models;

// Validators for bank API response types

public partial class OBWriteDomesticConsentResponse5 : ISupportsValidation
{
    public async Task<ValidationResult> ValidateAsync() =>
        await Task.FromResult(new ValidationResult());
}

public partial class OBWriteFundsConfirmationResponse1 : ISupportsValidation
{
    public async Task<ValidationResult> ValidateAsync() =>
        await Task.FromResult(new ValidationResult());
}

public partial class OBWriteDomesticResponse5 : ISupportsValidation
{
    public async Task<ValidationResult> ValidateAsync() =>
        await Task.FromResult(new ValidationResult());
}