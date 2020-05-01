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
    public class SoftwareStatementProfileDbRepoTests : DbTest
    {
        private readonly DbEntityRepository<SoftwareStatementProfile> _repo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly ITestOutputHelper _output;

        public SoftwareStatementProfileDbRepoTests(ITestOutputHelper output)
        {
            _repo = new DbEntityRepository<SoftwareStatementProfile>(_dB);
            _dbMultiEntityMethods = new DbMultiEntityMethods(_dB);
            _output = output;
        }

        /// <summary>
        /// Confirm operation of generation of non-null strings without null chars
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        public bool FsCheckStringNotNullAndContainsNoNulls_WorksCorrectly(StringNotNullAndContainsNoNulls s)
        {
            bool? outcome = !s.Item?.Contains("\0");
            return outcome ?? false;
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
        public Property GetAsync_KnownId_ReturnsItem(StringNotNullAndContainsNoNulls id)
        {
            Func<bool> rule = () =>
            {
                SoftwareStatementProfile value = new SoftwareStatementProfile
                {
                    Id = id.Item
                };
                SoftwareStatementProfile _ = _repo.UpsertAsync(value).GetAwaiter().GetResult();
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                return _repo.GetAsync(id.Item).Result.Id == id.Item;
            };

            return rule.ToProperty();
        }

        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        public Property SetAsync_KnownId_ElementReplaced(StringNotNullAndContainsNoNulls id, StringNotNullAndContainsNoNulls url1, StringNotNullAndContainsNoNulls url2)
        {
            Func<bool> rule = () =>
            {
                SoftwareStatementProfile value = new SoftwareStatementProfile { Id = id.Item, DefaultFragmentRedirectUrl = url1.Item };
                SoftwareStatementProfile value2 = new SoftwareStatementProfile { Id = id.Item, DefaultFragmentRedirectUrl = url2.Item };

                SoftwareStatementProfile _ = _repo.UpsertAsync(value).Result;
                SoftwareStatementProfile __ = _repo.UpsertAsync(value2).Result;
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                SoftwareStatementProfile item = _repo.GetAsync(id.Item).Result;

                return item.Id == id.Item && item.DefaultFragmentRedirectUrl == value2.DefaultFragmentRedirectUrl;
            };

            // Run test avoiding C null character
            return rule.When(url1 != url2);
        }

        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        public Property DeleteAsync_KnownId_ReturnsItem(StringNotNullAndContainsNoNulls id)
        {
            Func<bool> rule = () =>
            {
                SoftwareStatementProfile value = new SoftwareStatementProfile
                {
                    Id = id.Item
                };
                SoftwareStatementProfile _ = _repo.UpsertAsync(value).Result;
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                _repo.RemoveAsync(value).Wait();
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                return _repo.GetAsync(id.Item).Result == null;
            };

            return rule.ToProperty();
        }
    }
}
