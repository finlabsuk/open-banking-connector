// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping
{
    public class EntityMapper : IEntityMapper
    {
        private readonly Lazy<IMapper> _mapper;

        public EntityMapper()
        {
            _mapper = new Lazy<IMapper>(() => CreateMapperConfiguration().CreateMapper());
        }


        public object Map(object value, Type targetType)
        {
            value.ArgNotNull(nameof(value));
            targetType.ArgNotNull(nameof(targetType));

            return _mapper.Value.Map(source: value, sourceType: value.GetType(), destinationType: targetType);
        }

        public TResult Map<TResult>(object value)
            where TResult : class
        {
            value.ArgNotNull(nameof(value));

            return _mapper.Value.Map(
                source: value,
                sourceType: value.GetType(),
                destinationType: typeof(TResult)) as TResult;
        }

        private MapperConfiguration CreateMapperConfiguration()
        {
            return new MapperConfiguration(cfg => { ApplyDirectReferenceTypeMaps(cfg); });
        }

        private void ApplyDirectReferenceTypeMaps(IMapperConfigurationExpression config)
        {
            EntityTypeFinder typeFinder = new EntityTypeFinder();
            IEnumerable<Type> publicTypes = typeFinder.GetPublicReferenceTypes();

            IEnumerable<EquivalentType> obTypePairs =
                publicTypes.SelectMany(t => typeFinder.GetOpenBankingEquivalentTypes(t));
            IEnumerable<EquivalentType> sourceApiEquivalentTypePairs =
                publicTypes.SelectMany(t => typeFinder.GetSourceApiEquivalentTypes(t));
            IEnumerable<EquivalentType> persistenceTypePairs =
                publicTypes.SelectMany(t => typeFinder.GetPersistenceEquivalentTypes(t));
            IEnumerable<EquivalentType> typePairs =
                obTypePairs.Concat(sourceApiEquivalentTypePairs).Concat(persistenceTypePairs);

            foreach (EquivalentType typePair in typePairs)
            {
                if (typePair is MappedEquivalentType tp)
                {
                    config.CreateMap(sourceType: tp.EntityType, destinationType: tp.EquivalentEntityType)
                        .ConvertUsing(tp.Mapper);
                }
                else
                {
                    config.CreateMap(sourceType: typePair.EntityType, destinationType: typePair.EquivalentEntityType);
                }
            }
        }
    }
}
