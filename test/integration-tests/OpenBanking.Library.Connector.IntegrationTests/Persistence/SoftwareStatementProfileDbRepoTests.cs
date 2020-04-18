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
    public class SoftwareStatementProfileDbRepoTests: DbTest
    {
        private readonly DbEntityRepository<SoftwareStatementProfile> _repo;
        private readonly ITestOutputHelper _output;

        public SoftwareStatementProfileDbRepoTests(ITestOutputHelper output)
        {
            _repo =  new DbEntityRepository<SoftwareStatementProfile>(_dB);
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

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetAsync_KnownId_ReturnsItem(string id)
        {
            Func<bool> rule = () =>
            {
                var value = new SoftwareStatementProfile();
                value.Id = id;
                var _ = _repo.SetAsync(value).GetAwaiter().GetResult();
                _repo.SaveChangesAsync().Wait();

                return _repo.GetAsync(id).Result.Id == id;
            };

            // Run test avoiding C null character
            if ( !(id is null) && id.Contains("\0"))
            {
                return true.ToProperty();
            } else
            {
                return rule.When(id != null);
            }
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property SetAsync_KnownId_ElementReplaced(string id, string url1, string url2)
        {
            Func<bool> rule = () =>
            {
                var value = new SoftwareStatementProfile { Id = id, DefaultFragmentRedirectUrl = url1 };
                var value2 = new SoftwareStatementProfile { Id = id, DefaultFragmentRedirectUrl = url2 };

                var _ = _repo.SetAsync(value).Result;
                var __ = _repo.SetAsync(value2).Result;
                _repo.SaveChangesAsync().Wait();

                var item = _repo.GetAsync(id).Result;

                return item.Id == id && item.DefaultFragmentRedirectUrl == value2.DefaultFragmentRedirectUrl;
            };
            
            // Run test avoiding C null character
            if ( !(id is null) && id.Contains("\0"))
            {
                return true.ToProperty();
            } else
            {
                return rule.When(id != null && url1 != null && url2 != null && url1 != url2);
            }
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property DeleteAsync_KnownId_ReturnsItem(string id)
        {
            Func<bool> rule = () =>
            {
                var value = new SoftwareStatementProfile();
                value.Id = id;
                var _ = _repo.SetAsync(value).Result;
                _repo.SaveChangesAsync().Wait();

                _repo.DeleteAsync(value).Wait();
                _repo.SaveChangesAsync().Wait();

                return _repo.GetAsync(id).Result == null;
            };

            return rule.When(id != null);
        }
    }
}
