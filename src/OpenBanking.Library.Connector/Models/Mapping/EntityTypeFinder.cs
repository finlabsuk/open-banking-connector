// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using Meta = FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model.Meta;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping
{
    internal class EntityTypeFinder
    {
        private static readonly Type RootPublicType = typeof(ModelFactory);

        public IEnumerable<Type> GetPublicReferenceTypes()
        {
            return RootPublicType.Assembly.GetTypes().Where(t => t.IsClass && IsInNamespace(RootPublicType, t));
        }


        public IEnumerable<EquivalentType> GetOpenBankingEquivalentTypes(Type type)
        {
            var attrs = type.ArgNotNull(nameof(type))
                .GetCustomAttributes(typeof(OpenBankingEquivalentAttribute))
                .OfType<OpenBankingEquivalentAttribute>();

            foreach (var attr in attrs)
            {
                yield return attr.EquivalentTypeMapper == null
                    ? new EquivalentType(type, attr.EquivalentType)
                    : new MappedEquivalentType(type, attr.EquivalentType, attr.EquivalentTypeMapper);
            }
        }

        public IEnumerable<EquivalentType> GetPersistenceEquivalentTypes(Type type)
        {
            var attrs = type.ArgNotNull(nameof(type))
                .GetCustomAttributes(typeof(PersistenceEquivalentAttribute))
                .OfType<PersistenceEquivalentAttribute>();


            foreach (var attr in attrs)
            {
                yield return attr.EquivalentTypeMapper == null
                    ? new EquivalentType(type, attr.EquivalentType)
                    : new MappedEquivalentType(type, attr.EquivalentType, attr.EquivalentTypeMapper);
            }
        }

        private bool IsInNamespace(Type rootType, Type type)
        {
            return rootType.Namespace != null && type.Namespace.Maybe(n => n.StartsWith(rootType.Namespace));
        }

        private Type GetRootType(ApiVersion version)
        {
            switch (version)
            {
                case ApiVersion.V3P1P1:
                    return typeof(Meta);

                case ApiVersion.V3P1P2:
                    return typeof(ObModels.PaymentInitiation.V3p1p2.Model.Meta);

                default:
                    throw new NotSupportedException($"The version {version} is not supported.");
            }
        }
    }
}
