// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V4p0.NSwagVrp.Models;

// Validators for bank API response types

public partial record OBDomesticVRPConsentResponse : ISupportsValidation
{
    public async Task<ValidationResult> ValidateAsync() =>
        await Task.FromResult(new ValidationResult());
}

public partial record OBVRPFundsConfirmationResponse : ISupportsValidation
{
    public async Task<ValidationResult> ValidateAsync() =>
        await Task.FromResult(new ValidationResult());
}

public partial record OBDomesticVRPResponse : ISupportsValidation
{
    public async Task<ValidationResult> ValidateAsync() =>
         await Task.FromResult(new ValidationResult());
}

public partial record OBDomesticVRPDetails : ISupportsValidation
{
    public async Task<ValidationResult> ValidateAsync() =>
        await Task.FromResult(new ValidationResult());
}