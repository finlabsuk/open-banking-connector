// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.BankApiModels;

namespace FinnovationLabs.OpenBanking.Library.Connector.Mapping
{
    public class TypeMapping
    {
        public TypeMapping(
            Type sourceType,
            Type destinationType,
            Type? typeConverter,
            string?[]? valueMappingSourceMembers,
            string[]? valueMappingDestinationMembers,
            ValueMapping[]? valueMappings)
        {
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
            TypeConverter = typeConverter;

            // Only set ValueConverters when inputs valid
            if (valueMappings is null ||
                valueMappingSourceMembers is null ||
                valueMappingDestinationMembers is null ||
                valueMappingSourceMembers.Length != valueMappings.Length ||
                valueMappingDestinationMembers.Length != valueMappings.Length)
            {
                if (valueMappingSourceMembers is null &&
                    valueMappingDestinationMembers is null &&
                    valueMappings is null) { }
                else
                {
                    throw new ArgumentException(
                        $"Can't create mapping from ${sourceType} to ${destinationType} due to invalid settings for Value Converters");
                }

                ValueConverters =
                    new List<(string? SourceMember, string DestinationMember, ValueMapping ValueConverter)>();
            }
            else
            {
                ValueConverters = Enumerable.Range(0, valueMappingDestinationMembers.Length)
                    .Select(i => (valueMappingSourceMembers[i], valueMappingDestinationMembers[i], valueMappings[i]))
                    .ToList();
            }
        }

        public Type SourceType { get; }
        public Type DestinationType { get; }
        public Type? TypeConverter { get; }

        public List<(string? SourceMember, string DestinationMember, ValueMapping ValueConverter)>
            ValueConverters { get; }
    }
}
