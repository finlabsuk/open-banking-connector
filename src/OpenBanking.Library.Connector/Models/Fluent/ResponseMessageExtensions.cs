// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public static class ResponseMessageExtensions
    {
        public static FluentResponseMessage CreateErrorMessage(this Exception exception)
        {
            return new FluentResponseErrorMessage(exception);
        }

        public static IList<FluentResponseMessage> CreateErrorMessages(this AggregateException exception)
        {
            return exception.InnerExceptions.Select(e => new FluentResponseErrorMessage(e))
                .OfType<FluentResponseMessage>()
                .ToArray();
        }
    }
}
