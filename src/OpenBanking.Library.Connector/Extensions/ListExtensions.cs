// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Extensions;

public static class ListExtensions
{
    public static void AddRange<T>(this IList<T> input, IEnumerable<T> elements)
    {
        if (input is List<T> inputAsList)
        {
            inputAsList.AddRange(elements);
        }
        else
        {
            foreach (T element in elements)
            {
                input.Add(element);
            }
        }
    }
}
