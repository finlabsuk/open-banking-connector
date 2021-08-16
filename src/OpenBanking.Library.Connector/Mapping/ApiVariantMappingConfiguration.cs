// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.Converters;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.UkRwApi.V3p1p6.PaymentInitiation.Models;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.UkDcrApi.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Mapping
{
    internal class ApiVariantMappingConfiguration
    {
        private static readonly Type ClientRegistrationModelsRootType =
            typeof(ClientRegistrationModelsPublic.OBClientRegistration1);

        private static readonly Type PaymentInitiationModelsRootType = typeof(PaymentInitiationModelsPublic.Meta);

        public IEnumerable<Type> GetPublicApiModelsTypes()
        {
            IEnumerable<Type> clientRegistrationTypes = ClientRegistrationModelsRootType.Assembly.GetTypes()
                .Where(t => t.IsClass && IsInNamespace(ClientRegistrationModelsRootType, t));

            IEnumerable<Type> paymentInitiationTypes = PaymentInitiationModelsRootType.Assembly.GetTypes()
                .Where(t => t.IsClass && IsInNamespace(PaymentInitiationModelsRootType, t));
            return clientRegistrationTypes.Concat(paymentInitiationTypes);
        }

        public IEnumerable<TypeMapping> GetTypesWithTargetApiEquivalent(Type type)
        {
            IEnumerable<TargetApiEquivalentAttribute> attrs = type.ArgNotNull(nameof(type))
                .GetCustomAttributes(typeof(TargetApiEquivalentAttribute))
                .OfType<TargetApiEquivalentAttribute>();

            foreach (TargetApiEquivalentAttribute attr in attrs)
            {
                yield return new TypeMapping(
                    type,
                    attr.EquivalentType,
                    attr.TypeConverter,
                    attr.ValueMappingSourceMembers,
                    attr.ValueMappingDestinationMembers,
                    attr.ValueMappings);
            }
        }

        public IEnumerable<TypeMapping> GetTypesWithSourceApiEquivalent(Type type)
        {
            IEnumerable<SourceApiEquivalentAttribute> attrs = type.ArgNotNull(nameof(type))
                .GetCustomAttributes(typeof(SourceApiEquivalentAttribute))
                .OfType<SourceApiEquivalentAttribute>();

            foreach (SourceApiEquivalentAttribute attr in attrs)
            {
                yield return new TypeMapping(
                    attr.EquivalentType,
                    type,
                    attr.TypeConverter,
                    attr.ValueMappingSourceMembers,
                    attr.ValueMappingDestinationMembers,
                    attr.ValueMappings);
            }
        }

        public MapperConfiguration CreateMapperConfiguration()
        {
            return new MapperConfiguration(
                cfg =>
                {
                    // Faithfully map null collections
                    cfg.AllowNullCollections = true;

                    // Generic mappings
                    ApplyGenericTypeMaps(cfg);

                    // Discovered mappings
                    ApplyApiVariantTypeMaps(cfg);
                });
        }

        private void ApplyGenericTypeMaps(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<string, IEnumerable<string>>().ConvertUsing<StringToIEnumerableConverter>();
            cfg.CreateMap<IEnumerable<string>, string>().ConvertUsing<StringToIEnumerableReverseConverter>();
        }

        private void ApplyApiVariantTypeMaps(IMapperConfigurationExpression config)
        {
            IEnumerable<Type> publicModelsTypes = GetPublicApiModelsTypes();

            IEnumerable<TypeMapping> typePairs =
                publicModelsTypes.SelectMany(
                    t =>
                    {
                        IEnumerable<TypeMapping> x = GetTypesWithTargetApiEquivalent(t);
                        IEnumerable<TypeMapping> y = GetTypesWithSourceApiEquivalent(t);
                        return x.Concat(y);
                    });

            foreach (TypeMapping typeMapping in typePairs)
            {
                IMappingExpression mappingExpression = config.CreateMap(
                    typeMapping.SourceType,
                    typeMapping.DestinationType);

                if (!(typeMapping.TypeConverter is null))
                {
                    mappingExpression.ConvertUsing(typeMapping.TypeConverter);
                }

                if (typeMapping.ValueConverters.Any())
                {
                    foreach ((string? sourceMember, string destinationMember,
                        ValueMapping valueMapping) in typeMapping
                        .ValueConverters)
                    {
                        mappingExpression.ForMember(
                            destinationMember,
                            valueMapping switch
                            {
                                ValueMapping.SetNull => delegate(IMemberConfigurationExpression opt)
                                {
                                    opt.MapFrom(src => (object?) null);
                                },
                                ValueMapping.CommaDelimitedStringToIEnumerable => delegate(
                                    IMemberConfigurationExpression opt)
                                {
                                    opt.ConvertUsing(
                                        new CommaDelimitedStringToIEnumerableValueConverter(),
                                        sourceMember);
                                },
                                ValueMapping.CommaDelimitedStringToIEnumerableReverse => delegate(
                                    IMemberConfigurationExpression opt)
                                {
                                    opt.ConvertUsing(
                                        new CommaDelimitedStringToIEnumerableReverseValueConverter(),
                                        sourceMember);
                                },
                                _ => throw new ArgumentOutOfRangeException()
                            });
                    }
                }
            }
        }

        private bool IsInNamespace(Type rootType, Type type)
        {
            return rootType.Namespace != null && type.Namespace.Maybe(n => n.StartsWith(rootType.Namespace));
        }
    }
}
