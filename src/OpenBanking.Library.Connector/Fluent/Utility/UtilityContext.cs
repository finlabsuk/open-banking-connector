// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Utility;

public interface IUtilityContext
{
    /// <summary>
    ///     Map from one Open Banking type to another.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDest"></typeparam>
    void Map<TSource, TDest>(TSource source, out TDest dest)
        where TSource : class
        where TDest : class;
}

internal class UtilityContext : IUtilityContext
{
    private readonly ISharedContext _sharedContext;


    public UtilityContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
    }


    public void Map<TSource, TDest>(TSource source, out TDest dest)
        where TSource : class
        where TDest : class =>
        _sharedContext.ApiVariantMapper.Map(source, out dest);
}
