// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping
{
    internal class EquivalentType
    {
        public EquivalentType(Type entityType, Type equivalentEntityType)
        {
            EntityType = entityType.ArgNotNull(nameof(entityType));
            EquivalentEntityType = equivalentEntityType.ArgNotNull(nameof(equivalentEntityType));
        }

        public Type EntityType { get; }
        public Type EquivalentEntityType { get; }
    }

    internal class MappedEquivalentType : EquivalentType
    {
        public MappedEquivalentType(Type entityType, Type equivalentEntityType, Type mapper) : base(entityType,
            equivalentEntityType)
        {
            Mapper = mapper.ArgNotNull(nameof(mapper));
        }

        public Type Mapper { get; }
    }
}
