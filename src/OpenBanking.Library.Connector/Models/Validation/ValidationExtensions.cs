// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation
{
    public static class ValidationExtensions
    {
        public static IList<FluentResponseMessage> GetOpenBankingResponses(this ValidationResult validationResult)
        {
            List<FluentResponseMessage> result = validationResult.Errors
                .Select(e => new FluentResponseErrorMessage(e.ErrorMessage))
                .Cast<FluentResponseMessage>()
                .ToList();

            return result;
        }

        public static void RaiseErrorOnValidationError(this ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                IEnumerable<string> msgs = validationResult.Errors.Select(vf => vf.ErrorMessage);
                string msg = string.Join(separator: Environment.NewLine, values: msgs);

                throw new Exception(msg);
            }
        }
    }
}
