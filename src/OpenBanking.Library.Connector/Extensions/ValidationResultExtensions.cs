// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FluentValidation;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Extensions
{
    public static class ValidationResultExtensions
    {
        public static IEnumerable<IFluentResponseInfoOrWarningMessage>
            ProcessValidationResultsAndReturnBadRequestErrorMessages(
                this ValidationResult validationResult,
                string messagePrefix,
                out IList<FluentResponseBadRequestErrorMessage> badRequestErrorMessages)
        {
            // Collect error messages
            badRequestErrorMessages = validationResult.Errors
                .Where(failure => failure.Severity == Severity.Error)
                .Select(failure => new FluentResponseBadRequestErrorMessage($"{messagePrefix}:{failure.ErrorMessage}"))
                .ToList();

            // Pass non-error messages
            IEnumerable<IFluentResponseInfoOrWarningMessage> nonErrorMessages = validationResult.Errors
                .Where(x => x.Severity == Severity.Info || x.Severity == Severity.Warning)
                .Select(failure => failure.GetNonErrorMessage(messagePrefix));

            return nonErrorMessages;
        }

        private static IFluentResponseInfoOrWarningMessage GetNonErrorMessage(
            this ValidationFailure failure,
            string messagePrefix)
        {
            return failure.Severity switch
            {
                Severity.Warning => new FluentResponseWarningMessage($"{messagePrefix}:{failure.ErrorMessage}"),
                Severity.Info => new FluentResponseInfoMessage($"{messagePrefix}:{failure.ErrorMessage}"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static IEnumerable<IFluentResponseInfoOrWarningMessage> ProcessValidationResultsAndRaiseErrors(
            this ValidationResult validationResult,
            string messagePrefix,
            string topLevelMessage =
                "Validation failure when checking Open Banking API types. These checks are performed on outgoing and incoming data for every Open Banking API call.")
        {
            // If any error messages, throw aggregate exception with messages
            List<Exception> otherErrorExceptions = validationResult.Errors
                .Where(failure => failure.Severity == Severity.Error)
                .Select(failure => new Exception($"{messagePrefix}:{failure.ErrorMessage}"))
                .ToList();
            if (otherErrorExceptions.Any())
            {
                throw new AggregateException(
                    topLevelMessage,
                    otherErrorExceptions);
            }

            // Pass non-error messages
            IEnumerable<IFluentResponseInfoOrWarningMessage> nonErrorMessages = validationResult.Errors
                .Where(x => x.Severity == Severity.Info || x.Severity == Severity.Warning)
                .Select(failure => failure.GetNonErrorMessage(messagePrefix));

            return nonErrorMessages;
        }
    }
}
