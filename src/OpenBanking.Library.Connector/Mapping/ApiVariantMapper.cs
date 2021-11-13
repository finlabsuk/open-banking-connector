// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using AutoMapper;

namespace FinnovationLabs.OpenBanking.Library.Connector.Mapping
{
    /// <summary>
    ///     Wrapper for IMapper with convenience methods. Used to provide mapping
    ///     to API variants (different Open Banking standard versions).
    /// </summary>
    public class ApiVariantMapper : IApiVariantMapper
    {
        private readonly Lazy<IMapper> _mapper;

        public ApiVariantMapper()
        {
            _mapper = new Lazy<IMapper>(
                () =>
                {
                    var typeFinder = new ApiVariantMappingConfiguration();
                    return typeFinder.CreateMapperConfiguration().CreateMapper();
                });
        }

        /// <summary>
        ///     Wrapper for Map that allows type parameters to be inferred from arguments
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        public void Map<TInput, TResult>(TInput source, out TResult dest)
            where TInput : class
            where TResult : class
        {
            dest = _mapper.Value.Map<TInput, TResult>(source);
        }
    }
}
