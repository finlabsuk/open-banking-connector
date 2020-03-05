// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FsCheck;
using FsCheck.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Security
{
    public class MemoryOpenBankingClientProfileProfileRepositoryTests
    {
        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetAsync_ReturnsEmpty(string id)
        {
            Func<bool> rule = () =>
            {
                var repo = new MemoryOpenBankingClientProfileRepository();

                return repo.GetAsync(id).Result == null;
            };

            return rule.When(id != null);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetAsync_KnownId_ReturnsItem(string id, BankClientProfile value)
        {
            Func<bool> rule = () =>
            {
                var repo = new MemoryOpenBankingClientProfileRepository();

                value.Id = id;
                var _ = repo.SetAsync(value).Result;

                return repo.GetAsync(id).Result.Id == id;
            };

            return rule.When(id != null && value != null);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property SetAsync_KnownId_ElementReplaced(string id, BankClientProfile value,
            BankClientProfile value2)
        {
            Func<bool> rule = () =>
            {
                var repo = new MemoryOpenBankingClientProfileRepository();

                value.Id = id;
                value2.Id = id;

                var _ = repo.SetAsync(value).Result;
                var __ = repo.SetAsync(value2).Result;
                var item = repo.GetAsync(id).Result;

                return item.Id == id && item.XFapiFinancialId == value2.XFapiFinancialId;
            };

            return rule.When(id != null && value != null && value2 != null &&
                             value.XFapiFinancialId != value2.XFapiFinancialId);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property DeleteAsync_KnownId_ReturnsItem(string id, BankClientProfile value)
        {
            Func<bool> rule = () =>
            {
                var repo = new MemoryOpenBankingClientProfileRepository();

                value.Id = id;
                var _ = repo.SetAsync(value).Result;

                var result = repo.DeleteAsync(id).Result;

                return result && repo.GetAsync(id).Result == null;
            };

            return rule.When(id != null && value != null);
        }
    }
}
