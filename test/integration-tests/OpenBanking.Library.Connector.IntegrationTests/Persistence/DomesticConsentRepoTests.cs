// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Persistence
{
    public class DomesticConsentRepoTests: DbTest
    {
        private readonly IDbEntityRepository<DomesticConsent> _repo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly ITestOutputHelper _output;

        public DomesticConsentRepoTests(ITestOutputHelper output)
        {
            _repo = new DbEntityRepository<DomesticConsent>(_dB);
            _dbMultiEntityMethods = new DbMultiEntityMethods(_dB);
            _output = output;
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(11)]
        [InlineData(15)]
        public async Task GetAsync_ByExpression_GetByUniqueProperty(int count)
        {
            var items = Enumerable.Range(1, count)
                .Select(i => new DomesticConsent
                {
                    Id = i.ToString()
                }).ToList();
            foreach (var dc in items)
            {
                await _repo.UpsertAsync(dc);
            }
            _dbMultiEntityMethods.SaveChangesAsync().Wait();

            var q = await _repo.GetAsync(x => x.Id == count.ToString());

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
            var items = Enumerable.Range(1, count)
                .Select(i => new DomesticConsent
                {
                    Id = i.ToString()
                }).ToList();
            foreach (var dc in items)
            {
                await _repo.UpsertAsync(dc);
            }
            _dbMultiEntityMethods.SaveChangesAsync().Wait();

            var q = await _repo.GetAsync(x => true);

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
            var items = Enumerable.Range(1, count)
                .Select(i => new DomesticConsent
                {
                    Id = (-i).ToString()
                }).ToList();
            foreach (var dc in items)
            {
                await _repo.UpsertAsync(dc);
            }
            _dbMultiEntityMethods.SaveChangesAsync().Wait();

            var q = await _repo.GetAsync(x => x.Id == count.ToString());

            var results = q.ToList();

            results.Should().HaveCount(0);
        }
    }
}
