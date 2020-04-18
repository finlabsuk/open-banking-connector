// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Persistence
{
    public class MemoryDomesticConsentRepositoryTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(11)]
        [InlineData(15)]
        public async Task GetAsync_ByExpression_GetByUniqueProperty(int count)
        {
            var cache = new MemoryDomesticConsentRepository();
            var items = Enumerable.Range(1, count)
                .Select(i => new DomesticConsent
                {
                    Id = i.ToString()
                }).ToList();
            foreach (var dc in items)
            {
                await cache.SetAsync(dc);
            }

            var q = await cache.GetAsync(x => x.Id == count.ToString());

            var results = q.ToList();

            results.Should().HaveCount(1);
            results[0].Should().Be(items.Last());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(11)]
        [InlineData(15)]
        public async Task GetAsync_ByExpression_GetByAll(int count)
        {
            var cache = new MemoryDomesticConsentRepository();
            var items = Enumerable.Range(1, count)
                .Select(i => new DomesticConsent
                {
                    Id = i.ToString()
                }).ToList();
            foreach (var dc in items)
            {
                await cache.SetAsync(dc);
            }

            var q = await cache.GetAsync(x => true);

            var results = q.ToList();

            results.Should().BeEquivalentTo(items);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(11)]
        [InlineData(15)]
        public async Task GetAsync_ByExpression_GetEmptySetByUniqueProperty(int count)
        {
            var cache = new MemoryDomesticConsentRepository();
            var items = Enumerable.Range(1, count)
                .Select(i => new DomesticConsent
                {
                    Id = (-i).ToString()
                }).ToList();
            foreach (var dc in items)
            {
                await cache.SetAsync(dc);
            }

            var q = await cache.GetAsync(x => x.Id == count.ToString());

            var results = q.ToList();

            results.Should().HaveCount(0);
        }
    }
}
