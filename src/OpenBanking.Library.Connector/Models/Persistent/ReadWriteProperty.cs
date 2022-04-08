// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Mutable single property in DB.
    ///     Please note that properties are read-only so they are created via the
    ///     constructor only when the object is replaced and not adjusted one by one.
    ///     This means they must also be explicitly mapped in OnModelCreating().
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Owned]
    public class ReadWriteProperty<TValue>
    {
        /// <summary>
        ///     Constructor intended for use by EF Core only.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modified"></param>
        /// <param name="modifiedBy"></param>
        private protected ReadWriteProperty(TValue value, DateTimeOffset modified, string? modifiedBy)
        {
            Value = value;
            Modified = modified;
            ModifiedBy = modifiedBy;
        }

        public ReadWriteProperty(TValue value, ITimeProvider timeProvider, string? modifiedBy)
        {
            Value = value;
            Modified = timeProvider.GetUtcNow();
            ModifiedBy = modifiedBy;
        }

        public TValue Value { get; }

        public DateTimeOffset Modified { get; }

        public string? ModifiedBy { get; }
    }

    /// <summary>
    ///     Mutable property group in DB.
    ///     Please note that properties are read-only so they are created via the
    ///     constructor only when the object is replaced and not adjusted one by one.
    ///     This means they must also be explicitly mapped in OnModelCreating().
    /// </summary>
    /// <typeparam name="TValue1"></typeparam>
    /// <typeparam name="TValue2"></typeparam>
    [Owned]
    public class ReadWritePropertyGroup<TValue1, TValue2>
    {
        /// <summary>
        ///     Constructor intended for use by EF Core only.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="modified"></param>
        /// <param name="modifiedBy"></param>
        private protected ReadWritePropertyGroup(TValue1 value1, TValue2 value2, DateTimeOffset modified, string? modifiedBy)
        {
            Value1 = value1;
            Value2 = value2;
            Modified = modified;
            ModifiedBy = modifiedBy;
        }

        public ReadWritePropertyGroup(TValue1 value1, TValue2 value2, ITimeProvider timeProvider, string? modifiedBy)
        {
            Value1 = value1;
            Value2 = value2;
            Modified = timeProvider.GetUtcNow();
            ModifiedBy = modifiedBy;
        }

        public TValue1 Value1 { get; }

        public TValue2 Value2 { get; }

        public DateTimeOffset Modified { get; }

        public string? ModifiedBy { get; }
    }

}
