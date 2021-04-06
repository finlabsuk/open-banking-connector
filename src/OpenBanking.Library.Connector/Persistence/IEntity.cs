// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    public interface IEntity
    {
        Guid Id { get; }
        ReadWriteProperty<bool> IsDeleted { get; set; }
        DateTimeOffset Created { get; }
        string? CreatedBy { get; }
    }
}
