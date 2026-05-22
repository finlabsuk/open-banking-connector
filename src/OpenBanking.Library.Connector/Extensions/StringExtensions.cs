// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Text.RegularExpressions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Extensions;

public static class StringExtensions
{
    public static string PascalOrCamelToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        MatchCollection matches = Regex.Matches(
            value,
            @"\G(.[^A-Z]*)", // one possibly capital followed by optional non-capitals for each group
            RegexOptions.Compiled);
        IEnumerable<string> stringMatches = from Match match in matches select match.Value;
        string returnString = string.Join("-", stringMatches).ToLower();
        return returnString;
    }

    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value) ||
            char.IsLower(value[0]))
        {
            return value;
        }
        return char.ToLower(value[0]) + value[1..];
    }

    public static string FromPascalCaseToLowerWords(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var sb = new StringBuilder();
        foreach (char c in value)
        {
            if (char.IsUpper(c) &&
                sb.Length > 0)
            {
                sb.Append(' ');
            }
            sb.Append(char.ToLower(c));
        }
        return sb.ToString();
    }
}
