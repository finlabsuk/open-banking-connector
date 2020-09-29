// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    public interface IEntity
    {
        string Id { get; }
        ReadWriteProperty<bool> IsDeleted { get; set; }
        DateTimeOffset Created { get; }
        string? CreatedBy { get; }
    }
}
