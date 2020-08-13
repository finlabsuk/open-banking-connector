// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Extensions
{
    public static class StringExtensions
    {
        public static string PascalOrCamelToKebabCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            MatchCollection matches = Regex.Matches(
                input: value,
                pattern: @"\G(.[^A-Z]*)", // one possibly capital followed by optional non-capitals for each group
                options: RegexOptions.Compiled);
            IEnumerable<string> stringMatches = from Match match in matches select match.Value;
            string returnString = string.Join(separator: "-", values: stringMatches).ToLower();
            return returnString;
        }
    }
}
