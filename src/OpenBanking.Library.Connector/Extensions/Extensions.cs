// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Extensions;

internal static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Maybe<T, TResult>(this T? value, Func<T, TResult> selector)
        where T : class
        where TResult : struct =>
        value != null
            ? selector(value)
            : default;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T>? values) => values ?? Enumerable.Empty<T>();

    public static IEnumerable<T> WalkRecursive<T>(this T value, Func<T, T?> selector)
        where T : class =>
        WalkRecursiveInner(value, selector.ArgNotNull(nameof(selector)));


    public static Task<T> ToTaskResult<T>(this T value) => Task.FromResult(value);

    public static async Task<T[]> WaitAll<T>(this Task<T>[] tasks) => await Task.WhenAll(tasks);

    public static int DelimiterCount(this string value, char delimiter)
    {
        var count = 0;
        var x = 0;
        do
        {
            x = value.IndexOf(delimiter, x);
            if (x < 0)
            {
                return count;
            }

            count++;
            x++;
        } while (x < value.Length);

        return count;
    }

    // HACK: the repetitive reflection over types will incur performance costs in future - please optimise
    public static Dictionary<string, object?> ToObjectDictionary<T>(this T value)
        where T : class
    {
        value.ArgNotNull(nameof(value));

        PropertyInfo[] props = typeof(T).GetProperties(
            BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.FlattenHierarchy);

        var pairs = props
            .Select(
                p => new
                {
                    Prop = p.GetGetMethod(),
                    Attr = p.GetCustomAttribute<JsonPropertyAttribute>()
                })
            .Where(a => a.Attr?.PropertyName is not null && a.Prop is not null);

        var result = new Dictionary<string, object?>(StringComparer.InvariantCulture);
        foreach (var pair in pairs)
        {
            object? propValue = pair.Prop!.Invoke(value, null);

            result[pair.Attr?.PropertyName!] = propValue;
        }

        return result;
    }

    public static bool IsInNamespace(this Type type, Type rootType)
    {
        type.ArgNotNull(nameof(type));
        rootType.ArgNotNull(nameof(rootType));

        return rootType.Namespace != null &&
               type.Namespace.Maybe(n => n.StartsWith(rootType.Namespace));
    }

    public static string JoinString(this IEnumerable<string> lines, string delimiter) =>
        string.Join(delimiter, lines.ArgNotNull(nameof(lines)));


    public static string ToUrlParameterString(
        this IEnumerable<KeyValuePair<string, string>> values,
        bool doNotUseUrlPathEncoding = false) =>
        values.Select(
                kvp =>
                    $"{(doNotUseUrlPathEncoding ? kvp.Key : HttpUtility.UrlPathEncode(kvp.Key))}=" +
                    $"{(doNotUseUrlPathEncoding ? kvp.Value : HttpUtility.UrlPathEncode(kvp.Value))}")
            .JoinString("&");

    private static IEnumerable<T> WalkRecursiveInner<T>(T value, Func<T, T?> selector)
        where T : class
    {
        yield return value;

        while (selector(value) is not null)
        {
            value = selector(value)!;
            yield return value;
        }
    }
}
