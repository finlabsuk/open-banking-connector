﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FsCheck;
using FsCheck.Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Persistence
{
    public class BankClientProfileDbRepoTests : DbTest
    {
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly ITestOutputHelper _output;
        private readonly IDbEntityRepository<BankRegistration> _repo;

        public BankClientProfileDbRepoTests(ITestOutputHelper output)
        {
            _repo = new DbEntityRepository<BankRegistration>(_dB);
            _dbMultiEntityMethods = new DbMultiEntityMethods(_dB);
            _output = output;
        }

        /// <summary>
        ///     Confirm operation of generation of non-null strings without null chars
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
            Func<bool> rule = () => { return _repo.GetAsync(id).Result == null; };

            return rule.When(id != null);
        }

        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        public Property GetAsync_KnownId_ReturnsItem(StringNotNullAndContainsNoNulls id)
        {
            Func<bool> rule = () =>
            {
                BankRegistration value = new BankRegistration
                {
                    SoftwareStatementProfileId = "sdf",
                    OpenIdConfiguration = new OpenIdConfiguration(),
                    BankClientRegistrationRequestData = new OBClientRegistration1(),
                    BankClientRegistrationData = new OBClientRegistration1(),
                    Id = id.Item
                };
                BankRegistration _ = _repo.UpsertAsync(value).GetAwaiter().GetResult();
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                return _repo.GetAsync(id.Item).Result.Id == id.Item;
            };

            return rule.ToProperty();
        }

        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        public Property SetAsync_KnownId_ElementReplaced(
            StringNotNullAndContainsNoNulls id,
            BankRegistration value,
            BankRegistration value2)
        {
            Func<bool> rule = () =>
            {
                value.Id = id.Item;
                value2.Id = id.Item;

                BankRegistration _ = _repo.UpsertAsync(value).Result;
                BankRegistration __ = _repo.UpsertAsync(value2).Result;
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                BankRegistration item = _repo.GetAsync(id.Item).Result;

                return item.Id == id.Item && item.BankId == value2.BankId;
            };

            // Run test avoiding C null character
            return rule.When(
                value != null && value2 != null &&
                value.BankId != value2.BankId);
        }

        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        public Property DeleteAsync_KnownId_ReturnsItem(StringNotNullAndContainsNoNulls id, BankRegistration value)
        {
            Func<bool> rule = () =>
            {
                value.Id = id.Item;
                BankRegistration _ = _repo.UpsertAsync(value).Result;
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                _repo.RemoveAsync(value).Wait();
                _dbMultiEntityMethods.SaveChangesAsync().Wait();

                return _repo.GetAsync(id.Item).Result == null;
            };

            return rule.When(value != null);
        }
    }
}
