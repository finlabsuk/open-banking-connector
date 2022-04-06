// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Threading.Tasks;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels
{
    public interface ISupportsValidation
    {
        public Task<ValidationResult> ValidateAsync();
    }
}
