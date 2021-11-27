// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    internal class
        DomesticVrpConsentPost : ReadWritePost<DomesticVrpConsentPersisted,
            DomesticVrpConsent,
            DomesticVrpConsentResponse,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>
    {
        private readonly IDbReadOnlyEntityMethods<BankApiSet> _bankApiSetMethods;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationMethods;

        public DomesticVrpConsentPost(
            IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IDbReadOnlyEntityMethods<BankApiSet> bankApiSetMethods,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper)
        {
            _bankApiSetMethods = bankApiSetMethods;
            _bankRegistrationMethods = bankRegistrationMethods;
        }

        protected override string RelativePath => "/domestic-vrp-consents";

        protected override async
            Task<(VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest apiRequest, BankApiSet
                bankApiInformation, BankRegistration bankRegistration, string bankFinancialId,
                TokenEndpointResponse? userTokenEndpointResponse, List<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)> ApiPostRequestData(DomesticVrpConsent request)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data and checks
            Guid bankRegistrationId = request.BankRegistrationId;
            BankRegistration bankRegistration =
                await _bankRegistrationMethods
                    .DbSetNoTracking
                    .Include(o => o.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == bankRegistrationId) ??
                throw new KeyNotFoundException(
                    $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
            Guid bankApiInformationId = request.BankApiSetId;
            BankApiSet bankApiSet =
                await _bankApiSetMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == bankApiInformationId) ??
                throw new KeyNotFoundException(
                    $"No record found for BankApiInformation {bankApiInformationId} specified by request.");
            if (bankApiSet.BankId != bankRegistration.BankId)
            {
                throw new ArgumentException("BankRegistrationId and BankProfileId objects do not share same BankId.");
            }

            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            // Create request
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest apiRequest =
                request.OBDomesticVRPConsentRequest;

            return (apiRequest, bankApiSet, bankRegistration, bankFinancialId, null,
                nonErrorMessages);
        }
    }
}
