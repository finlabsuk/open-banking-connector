// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FsCheck;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests
{
    public class StringNotNullAndContainsNoNulls
    {
        public StringNotNullAndContainsNoNulls(string s)
        {
            Item = s;
        }

        public string Item { get; }
    }

    public class FsCheckCustomArbs
    {
        public static Arbitrary<StringNotNullAndContainsNoNulls> GetArbStringNotNullAndContainsNoNulls()
        {
            return Arb.Default.String()
                .Filter(s => !(s is null) && !s.Contains("\0"))
                .Convert(convertTo: s => new StringNotNullAndContainsNoNulls(s), convertFrom: ans => ans.Item);
        }
    }
}
