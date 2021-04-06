// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Mapping
{
    public interface IApiVariantMapper
    {
        void Map<TSource, TDest>(TSource source, out TDest dest)
            where TSource : class
            where TDest : class;
    }
}
