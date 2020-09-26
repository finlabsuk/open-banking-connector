// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    [Owned]
    public class ReadWriteProperty<TData>
    {
        /// <summary>
        ///     Constructor intended for use by EF Core only.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="modified"></param>
        /// <param name="modifiedBy"></param>
        private protected ReadWriteProperty(TData data, DateTimeOffset modified, string? modifiedBy)
        {
            Data = data;
            Modified = modified;
            ModifiedBy = modifiedBy;
        }

        public ReadWriteProperty(TData data, ITimeProvider timeProvider, string? modifiedBy)
        {
            Data = data;
            Modified = timeProvider.GetUtcNow();
            ModifiedBy = modifiedBy;
        }

        public TData Data { get; }

        public DateTimeOffset Modified { get; }

        public string? ModifiedBy { get; }
    }
}
