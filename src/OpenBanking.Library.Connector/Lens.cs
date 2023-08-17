// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FinnovationLabs.OpenBanking.Library.Connector;

internal class Lens<T, TValue>
{
    public Lens(Func<T, TValue> get, Action<T, TValue> set)
    {
        Get = get.ArgNotNull(nameof(get));
        Set = set.ArgNotNull(nameof(get));
    }

    public Func<T, TValue> Get { get; }
    public Action<T, TValue> Set { get; }
}

internal static class Lens
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public static Lens<T, TValue> Create<T, TValue>(Func<T, TValue> get, Action<T, TValue> set) => new(get, set);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public static TValue GetOrCreateDefault<T, TValue>(this T value, Lens<T, TValue> lens)
        where T : class
    {
        TValue v = lens.Get(value);
        if (v == null)
        {
            v = Activator.CreateInstance<TValue>();
            lens.Set(value, v);
        }

        return v;
    }
}
