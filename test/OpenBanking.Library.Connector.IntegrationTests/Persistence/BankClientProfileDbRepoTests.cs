// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FsCheck;
using FsCheck.Xunit;
using Xunit.Abstractions;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.UkDcrApi.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Persistence
{
    public class BankClientProfileDbRepoTests : DbTest
    {
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly ITestOutputHelper _output;
        private readonly IDbEntityMethods<BankRegistration> _repo;

        public BankClientProfileDbRepoTests(ITestOutputHelper output)
        {
            _repo = new DbEntityMethods<BankRegistration>(_dB);
            _dbSaveChangesMethod = new DbSaveChangesMethod(_dB);
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
            bool? outcome = !s.Item.Contains("\0");
            return outcome ?? false;
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetAsync_ReturnsEmpty(Guid id)
        {
            Func<bool> rule = () => { return _repo.GetNoTrackingAsync(id).Result == null; };

            return rule.When(id != null);
        }

        // [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        // public Property GetAsync_KnownId_ReturnsItem(Guid id)
        // {
        //     Func<bool> rule = () =>
        //     {
        //         BankRegistration value = new BankRegistration(
        //             new TimeProvider(),
        //             "sdf",
        //             new OpenIdConfiguration
        //             {
        //                 Issuer = "Issuer",
        //                 ResponseTypesSupported = new List<string>(),
        //                 ScopesSupported = new List<string>(),
        //                 ResponseModesSupported = new List<string>(),
        //                 TokenEndpoint = "endpoint",
        //                 AuthorizationEndpoint = "endpoint",
        //                 RegistrationEndpoint = "endpoint"
        //             },
        //             new ClientRegistrationModelsPublic.OBClientRegistration1(),
        //             Guid.NewGuid(),
        //             new ClientRegistrationModelsPublic.OBClientRegistration1(),
        //             null,
        //             null,
        //             ClientRegistrationApiVersion.V3p3);
        //         _repo.AddAsync(value).GetAwaiter();
        //         _dbMultiEntityMethods.SaveChangesAsync().Wait();
        //
        //         return _repo.GetAsync(id).Result?.Id == id;
        //     };
        //
        //     return rule.ToProperty();
        // }

        // [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        //  public Property SetAsync_KnownId_ElementReplaced(
        //      StringNotNullAndContainsNoNulls id,
        //      BankRegistration value,
        //      BankRegistration value2)
        //  {
        //      Func<bool> rule = () =>
        //      {
        //          value.Id = id.Item;
        //          value2.Id = id.Item;
        //
        //          _repo.AddAsync(value).GetAwaiter();
        //          _repo.AddAsync(value2).GetAwaiter();
        //          _dbMultiEntityMethods.SaveChangesAsync().Wait();
        //
        //          BankRegistration item = _repo.GetAsync(id.Item).Result;
        //
        //          return item.Id == id.Item && item.BankId == value2.BankId;
        //      };
        //
        //      // Run test avoiding C null character
        //      return rule.When(
        //          value != null && value2 != null &&
        //          value.BankId != value2.BankId);
        //  }

        //[Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(FsCheckCustomArbs) })]
        // public Property DeleteAsync_KnownId_ReturnsItem(StringNotNullAndContainsNoNulls id, BankRegistration value)
        // {
        //     Func<bool> rule = () =>
        //     {
        //         value.Id = id.Item;
        //         _repo.AddAsync(value).GetAwaiter();
        //         _dbMultiEntityMethods.SaveChangesAsync().Wait();
        //
        //         _repo.RemoveAsync(value).Wait();
        //         _dbMultiEntityMethods.SaveChangesAsync().Wait();
        //
        //         return _repo.GetAsync(id.Item).Result == null;
        //     };
        //
        //     return rule.When(value != null);
        // }
    }
}
