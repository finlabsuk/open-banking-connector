// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping
{
    public interface IEntityMapper
    {
        object Map(object value, Type targetType);

        TResult Map<TResult>(object value)
            where TResult : class;
    }
}
