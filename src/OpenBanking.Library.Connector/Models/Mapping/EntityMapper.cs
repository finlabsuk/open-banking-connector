// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using AutoMapper;

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

            return _mapper.Value.Map(value, value.GetType(), targetType);
        }

        public TResult Map<TResult>(object value)
            where TResult : class
        {
            value.ArgNotNull(nameof(value));

            return _mapper.Value.Map(value, value.GetType(), typeof(TResult)) as TResult;
        }

        private MapperConfiguration CreateMapperConfiguration()
        {
            return new MapperConfiguration(cfg => { ApplyDirectReferenceTypeMaps(cfg); });
        }

        private void ApplyDirectReferenceTypeMaps(IMapperConfigurationExpression config)
        {
            var typeFinder = new EntityTypeFinder();
            var publicTypes = typeFinder.GetPublicReferenceTypes();

            var obTypePairs = publicTypes.SelectMany(t => typeFinder.GetOpenBankingEquivalentTypes(t));
            var persistenceTypePairs = publicTypes.SelectMany(t => typeFinder.GetPersistenceEquivalentTypes(t));
            var typePairs = obTypePairs.Concat(persistenceTypePairs);

            foreach (var typePair in typePairs)
            {
                if (typePair is MappedEquivalentType tp)
                {
                    config.CreateMap(tp.EntityType, tp.EquivalentEntityType)
                        .ConvertUsing(tp.Mapper);
                }
                else
                {
                    config.CreateMap(typePair.EntityType, typePair.EquivalentEntityType);
                }

                config.CreateMap(typePair.EquivalentEntityType, typePair.EntityType);
            }
        }
    }
}
