// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    public interface IEntity
    {
        Guid Id { get; }
        string? Reference { get; }

        bool IsDeleted { get; }

        DateTimeOffset IsDeletedModified { get; }

        string? IsDeletedModifiedBy { get; }

        DateTimeOffset Created { get; }
        string? CreatedBy { get; }

        void UpdateIsDeleted(bool isDeleted, DateTimeOffset isDeletedModified, string? modifiedBy);
    }
}
