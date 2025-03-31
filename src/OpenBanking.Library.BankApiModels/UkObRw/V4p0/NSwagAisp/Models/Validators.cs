// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using Azure.Core;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V4p0.NSwagAisp.Models;

// Validators for bank API response types

public partial record OBReadAccount6 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial record OBReadDirectDebit2 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial record OBReadStandingOrder6 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial record OBReadBalance1 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial record OBReadParty3 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial record OBReadParty2 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial record OBReadTransaction6 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial record OBReadConsentResponse1 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());

    public void WriteObject(Utf8JsonWriter jsonWriter)
    {
        jsonWriter.WriteObjectValue(this);
    }
}