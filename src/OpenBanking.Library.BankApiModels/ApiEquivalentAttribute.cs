// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels
{
    public abstract class ApiEquivalentAttribute : Attribute
    {
        protected ApiEquivalentAttribute(Type equivalentType)
        {
            EquivalentType = equivalentType;
        }

        public Type EquivalentType { get; set; }

        public Type? TypeConverter { get; set; }

        /// <summary>
        ///     Source member array for value converters (source member may be null where not applicable).
        /// </summary>
        public string?[]? ValueMappingSourceMembers { get; set; }

        /// <summary>
        ///     Destination member array for value converters
        /// </summary>
        public string[]? ValueMappingDestinationMembers { get; set; }

        /// <summary>
        ///     Array of value converters to use in mapping. Note this array, <see cref="ValueMappingSourceMembers" /> and
        ///     <see cref="ValueMappingDestinationMembers" /> must all
        ///     either be null or the same length.
        /// </summary>
        public ValueMapping[]? ValueMappings { get; set; }
    }
}
