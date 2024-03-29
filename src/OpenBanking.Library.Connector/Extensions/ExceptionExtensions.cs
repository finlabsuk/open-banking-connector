﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Extensions;

public static class ExceptionExtensions
{
    public static FluentResponseOtherErrorMessage CreateOtherErrorMessage(this Exception exception) => new(exception);

    public static IList<FluentResponseOtherErrorMessage> CreateOtherErrorMessages(this AggregateException exception)
    {
        return exception.InnerExceptions.Select(e => new FluentResponseOtherErrorMessage(e))
            .ToList();
    }
}
