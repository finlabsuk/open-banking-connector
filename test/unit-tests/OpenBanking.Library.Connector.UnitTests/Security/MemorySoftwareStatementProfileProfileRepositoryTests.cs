// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FsCheck;
using FsCheck.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Security
{
    public class MemorySoftwareStatementProfileProfileRepositoryTests
    {
        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetAsync_ReturnsEmpty(string id)
        {
            Func<bool> rule = () =>
            {
                var repo = new MemorySoftwareStatementProfileRepository();

                return repo.GetAsync(id).Result == null;
            };

            return rule.When(id != null);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetAsync_KnownId_ReturnsItem(string id)
        {
            Func<bool> rule = () =>
            {
                var repo = new MemorySoftwareStatementProfileRepository();

                var value = new SoftwareStatementProfile();
                value.Id = id;
                var _ = repo.SetAsync(value).Result;

                return repo.GetAsync(id).Result.Id == id;
            };

            return rule.When(id != null);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property SetAsync_KnownId_ElementReplaced(string id, string url1, string url2)
        {
            Func<bool> rule = () =>
            {
                var repo = new MemorySoftwareStatementProfileRepository();

                var value = new SoftwareStatementProfile { Id = id, DefaultFragmentRedirectUrl = url1 };
                var value2 = new SoftwareStatementProfile { Id = id, DefaultFragmentRedirectUrl = url2 };

                var _ = repo.SetAsync(value).Result;
                var __ = repo.SetAsync(value2).Result;
                var item = repo.GetAsync(id).Result;

                return item.Id == id && item.DefaultFragmentRedirectUrl == value2.DefaultFragmentRedirectUrl;
            };

            return rule.When(id != null && url1 != null && url2 != null && url1 != url2);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property DeleteAsync_KnownId_ReturnsItem(string id)
        {
            Func<bool> rule = () =>
            {
                var repo = new MemorySoftwareStatementProfileRepository();

                var value = new SoftwareStatementProfile();
                value.Id = id;
                var _ = repo.SetAsync(value).Result;

                var result = repo.DeleteAsync(id).Result;

                return result && repo.GetAsync(id).Result == null;
            };

            return rule.When(id != null);
        }
    }
}
