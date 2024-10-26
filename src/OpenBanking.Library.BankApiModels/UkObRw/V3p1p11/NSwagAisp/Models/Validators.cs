// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using Azure.Core;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.NSwagAisp.Models;

// Validators for bank API response types

public partial class OBReadAccount6 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial class OBReadDirectDebit2 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial class OBReadStandingOrder6 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial class OBReadBalance1 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial class OBReadParty3 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial class OBReadParty2 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial class OBReadTransaction6 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

public partial class OBReadConsentResponse1 : ISupportsValidation
{
    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());

    public void WriteObject(Utf8JsonWriter jsonWriter)
    {
        jsonWriter.WriteObjectValue(this);
    }
}