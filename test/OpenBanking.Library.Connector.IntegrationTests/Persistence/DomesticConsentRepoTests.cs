﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Persistence
{
    public class DomesticConsentRepoTests : DbTest
    {
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly ITestOutputHelper _output;
        private readonly IDbEntityMethods<DomesticPaymentConsent> _repo;

        public DomesticConsentRepoTests(ITestOutputHelper output)
        {
            _repo = new DbEntityMethods<DomesticPaymentConsent>(_dB);
            _dbSaveChangesMethod = new DbSaveChangesMethod(_dB);
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
            var latestGuid = Guid.NewGuid();
            List<DomesticPaymentConsent> items = Enumerable.Range(1, count)
                .Select(
                    i =>
                    {
                        latestGuid = Guid.NewGuid();
                        return new DomesticPaymentConsent
                        {
                            State = new ReadWriteProperty<string>(
                                "sdf",
                                new TimeProvider(),
                                null),
                            BankApiInformationId = Guid.NewGuid(),
                            OBWriteDomesticConsent = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4(),
                            OBWriteDomesticConsentResponse =
                                new PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5(),
                            //Id = i.ToString()
                            Id = latestGuid
                        };
                    }).ToList();
            foreach (var dc in items)
            {
                await _repo.AddAsync(dc);
            }

            _dbSaveChangesMethod.SaveChangesAsync().Wait();

            IQueryable<DomesticPaymentConsent> q = await _repo.GetNoTrackingAsync(x => x.Id == latestGuid);

            List<DomesticPaymentConsent> results = q.ToList();

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
            var latestGuid = Guid.NewGuid();
            List<DomesticPaymentConsent> items = Enumerable.Range(1, count)
                .Select(
                    i =>
                    {
                        latestGuid = Guid.NewGuid();
                        return new DomesticPaymentConsent
                        {
                            State = new ReadWriteProperty<string>(
                                "sdf",
                                new TimeProvider(),
                                null),
                            BankApiInformationId = Guid.NewGuid(),
                            OBWriteDomesticConsent = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4(),
                            OBWriteDomesticConsentResponse =
                                new PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5(),
                            //Id = i.ToString()
                            Id = latestGuid
                        };
                    }).ToList();
            foreach (var dc in items)
            {
                await _repo.AddAsync(dc);
            }

            _dbSaveChangesMethod.SaveChangesAsync().Wait();

            IQueryable<DomesticPaymentConsent> q = await _repo.GetNoTrackingAsync(x => true);

            List<DomesticPaymentConsent> results = q.ToList();

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
            var latestGuid = Guid.NewGuid();
            List<DomesticPaymentConsent> items = Enumerable.Range(1, count)
                .Select(
                    i =>
                    {
                        latestGuid = Guid.NewGuid();
                        return new DomesticPaymentConsent
                        {
                            State = new ReadWriteProperty<string>(
                                "sdf",
                                new TimeProvider(),
                                null),
                            BankApiInformationId = Guid.NewGuid(),
                            OBWriteDomesticConsent = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4(),
                            OBWriteDomesticConsentResponse =
                                new PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5(),
                            //Id = (-i).ToString()
                            Id = latestGuid
                        };
                    }).ToList();
            foreach (var dc in items)
            {
                await _repo.AddAsync(dc);
            }

            _dbSaveChangesMethod.SaveChangesAsync().Wait();

            IQueryable<DomesticPaymentConsent> q = await _repo.GetNoTrackingAsync(x => x.Id == latestGuid);

            List<DomesticPaymentConsent> results = q.ToList();

            results.Should().HaveCount(0);
        }
    }
}
