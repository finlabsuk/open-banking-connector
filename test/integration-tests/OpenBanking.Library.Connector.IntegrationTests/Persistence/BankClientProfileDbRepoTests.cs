// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FsCheck;
using FsCheck.Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Persistence
{
    public class BankClientProfileDbRepoTests : DbTest
    {
        private readonly IDbEntityRepository<BankClientProfile> _repo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly ITestOutputHelper _output;

        public BankClientProfileDbRepoTests(ITestOutputHelper output)
        {
            _repo = new DbEntityRepository<BankClientProfile>(_dB);
            _dbMultiEntityMethods = new DbMultiEntityMethods(_dB);
            _output = output;
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetAsync_ReturnsEmpty(string id)
        {
            Func<bool> rule = () =>
            {
                return _repo.GetAsync(id).Result == null;
            };

            return rule.When(id != null);
        }

        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        public Property GetAsync_KnownId_ReturnsItem(StringNotNullAndContainsNoNulls id, BankClientProfile value)
        {
            Func<bool> rule = () =>
            {
                value.Id = id.Item;
                BankClientProfile _ = _repo.UpsertAsync(value).GetAwaiter().GetResult();
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                return _repo.GetAsync(id.Item).Result.Id == id.Item;
            };

            // Run test avoiding C null character
            return rule.When(value != null);
        }

        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        public Property SetAsync_KnownId_ElementReplaced(StringNotNullAndContainsNoNulls id, BankClientProfile value,
            BankClientProfile value2)
        {
            Func<bool> rule = () =>
            {
                value.Id = id.Item;
                value2.Id = id.Item;

                BankClientProfile _ = _repo.UpsertAsync(value).Result;
                BankClientProfile __ = _repo.UpsertAsync(value2).Result;
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                BankClientProfile item = _repo.GetAsync(id.Item).Result;

                return item.Id == id.Item && item.XFapiFinancialId == value2.XFapiFinancialId;
            };

            // Run test avoiding C null character
            return rule.When(value != null && value2 != null &&
                                             value.XFapiFinancialId != value2.XFapiFinancialId);
        }

        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        public Property DeleteAsync_KnownId_ReturnsItem(StringNotNullAndContainsNoNulls id, BankClientProfile value)
        {
            Func<bool> rule = () =>
            {
                value.Id = id.Item;
                BankClientProfile _ = _repo.UpsertAsync(value).Result;
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                _repo.RemoveAsync(value).Wait();
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                return _repo.GetAsync(id.Item).Result == null;
            };

            return rule.When(value != null);
        }
    }
}
