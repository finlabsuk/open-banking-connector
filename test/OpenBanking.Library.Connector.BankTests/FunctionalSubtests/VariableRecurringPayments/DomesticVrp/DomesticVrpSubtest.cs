﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FluentAssertions;
using Jering.Javascript.NodeJS;
using DomesticVrpConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrpConsent;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.
    DomesticVrp
{
    public class DomesticVrpSubtest
    {
        public static ISet<DomesticVrpSubtestEnum>
            DomesticVrpFunctionalSubtestsSupported(BankProfile bankProfile) =>
            DomesticVrpSubtestHelper.AllDomesticVrpSubtests;

        public static async Task RunTest(
            DomesticVrpSubtestEnum subtestEnum,
            BankProfile bankProfile,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            VariableRecurringPaymentsApiSettings variableRecurringPaymentsApiSettings,
            IRequestBuilder requestBuilderIn,
            Func<IRequestBuilderContainer> requestBuilderGenerator,
            string testNameUnique,
            FilePathBuilder testDataProcessorFluentRequestLogging,
            bool includeConsentAuth,
            INodeJSService? nodeJsService,
            PuppeteerLaunchOptionsJavaScript? puppeteerLaunchOptions,
            List<BankUser> bankUserList)
        {
            bool subtestSkipped = subtestEnum switch
            {
                DomesticVrpSubtestEnum.VrpWithDebtorAccountSpecifiedByPisp => false,
                _ => throw new ArgumentException(
                    $"{nameof(subtestEnum)} is not valid {nameof(DomesticVrpSubtestEnum)} or needs to be added to this switch statement.")
            };
            if (subtestSkipped)
            {
                return;
            }

            // For now, we just use first bank user in list. Maybe later we can use different users for
            // different sub-tests.
            BankUser bankUser = bankUserList[0];

            IRequestBuilder requestBuilder = requestBuilderIn;

            // Basic request object for domestic payment consent
            DomesticVrpConsentRequest domesticVrpConsentRequest =
                bankProfile.DomesticVrpConsentRequest(
                    Guid.Empty,
                    Guid.Empty,
                    DomesticVrpSubtestHelper.DomesticVrpType(subtestEnum),
                    "placeholder: random GUID",
                    "placeholder: random GUID",
                    null);
            await testDataProcessorFluentRequestLogging
                .AppendToPath("domesticVrpConsent")
                .AppendToPath("postRequest")
                .WriteFile(domesticVrpConsentRequest);


            // Basic request object for domestic payment
            requestBuilder.Utility.Map(
                domesticVrpConsentRequest.OBDomesticVRPConsentRequest,
                out VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest obDomesticVrpRequest);
            DomesticVrpRequest domesticVrpRequest =
                new DomesticVrpRequest
                {
                    Name = null,
                    OBDomesticVRPRequest = obDomesticVrpRequest,
                    DomesticVrpConsentId = default
                };

            // POST domestic payment consent
            domesticVrpConsentRequest.BankRegistrationId = bankRegistrationId;
            domesticVrpConsentRequest.BankApiSetId = bankApiSetId;
            domesticVrpConsentRequest.Name = testNameUnique;
            IFluentResponse<DomesticVrpConsentResponse> domesticVrpConsentResp =
                await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                    .PostAsync(domesticVrpConsentRequest);

            // Checks
            domesticVrpConsentResp.Should().NotBeNull();
            domesticVrpConsentResp.Messages.Should().BeEmpty();
            domesticVrpConsentResp.Data.Should().NotBeNull();
            Guid domesticVrpConsentId = domesticVrpConsentResp.Data!.Id;

            // GET domestic payment consent
            IFluentResponse<DomesticVrpConsentResponse> domesticVrpConsentResp2 =
                await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                    .GetAsync(domesticVrpConsentId);

            // Checks
            domesticVrpConsentResp2.Should().NotBeNull();
            domesticVrpConsentResp2.Messages.Should().BeEmpty();
            domesticVrpConsentResp2.Data.Should().NotBeNull();

            // POST auth context
            var authContextRequest = new DomesticVrpConsentAuthContext
            {
                DomesticVrpConsentId = domesticVrpConsentId,
                Name = testNameUnique
            };
            IFluentResponse<DomesticVrpConsentAuthContextPostResponse> authContextResponse =
                await requestBuilder.VariableRecurringPayments
                    .DomesticVrpConsents
                    .AuthContexts
                    .PostLocalAsync(authContextRequest);

            // Checks
            authContextResponse.Should().NotBeNull();
            authContextResponse.Messages.Should().BeEmpty();
            authContextResponse.Data.Should().NotBeNull();
            authContextResponse.Data!.AuthUrl.Should().NotBeNull();
            Guid authContextId = authContextResponse.Data!.Id;
            string authUrl = authContextResponse.Data!.AuthUrl!;

            // GET auth context
            IFluentResponse<DomesticVrpConsentAuthContextResponse> authContextResponse2 =
                await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                    .AuthContexts
                    .GetLocalAsync(authContextId);
            // Checks
            authContextResponse2.Should().NotBeNull();
            authContextResponse2.Messages.Should().BeEmpty();
            authContextResponse2.Data.Should().NotBeNull();

            // Consent authorisation
            if (includeConsentAuth)
            {
                if (puppeteerLaunchOptions is null ||
                    nodeJsService is null)
                {
                    throw new ArgumentNullException($"{nameof(puppeteerLaunchOptions)} or {nameof(nodeJsService)}");
                }

                // Call Node JS to authorise consent in UI via Puppeteer
                object[] args =
                {
                    authUrl,
                    bankProfile.BankProfileEnum.ToString(),
                    "DomesticVrpConsent",
                    bankUser,
                    puppeteerLaunchOptions
                };
                await nodeJsService.InvokeFromFileAsync(
                    "authoriseConsent.js",
                    "authoriseConsent",
                    args);

                // Refresh scope to ensure user token acquired following consent is available
                using IRequestBuilderContainer scopedRequestBuilderNew = requestBuilderGenerator();
                IRequestBuilder requestBuilderNew = scopedRequestBuilderNew.RequestBuilder;

                if (variableRecurringPaymentsApiSettings.UseConsentGetFundsConfirmationEndpoint)
                {
                    // GET consent funds confirmation
                    IFluentResponse<DomesticVrpConsentResponse> domesticPaymentConsentResp4 =
                        await requestBuilderNew.VariableRecurringPayments.DomesticVrpConsents
                            .GetFundsConfirmationAsync(domesticVrpConsentId);

                    // Checks
                    domesticPaymentConsentResp4.Should().NotBeNull();
                    domesticPaymentConsentResp4.Messages.Should().BeEmpty();
                    domesticPaymentConsentResp4.Data.Should().NotBeNull();
                }

                // POST domestic payment
                await testDataProcessorFluentRequestLogging
                    .AppendToPath("domesticVrp")
                    .AppendToPath("postRequest")
                    .WriteFile(domesticVrpRequest);
                domesticVrpRequest.DomesticVrpConsentId = domesticVrpConsentId;
                domesticVrpRequest.Name = testNameUnique;
                IFluentResponse<DomesticVrpResponse>? domesticVrpResp =
                    await requestBuilderNew.VariableRecurringPayments.DomesticVrps
                        .PostAsync(domesticVrpRequest);

                // Checks
                domesticVrpResp.Should().NotBeNull();
                domesticVrpResp.Messages.Should().BeEmpty();
                domesticVrpResp.Data.Should().NotBeNull();
                Guid domesticVrpId = domesticVrpResp.Data!.Id;

                // GET domestic payment
                IFluentResponse<DomesticVrpResponse> domesticVrpResp2 =
                    await requestBuilderNew.VariableRecurringPayments.DomesticVrps
                        .GetAsync(domesticVrpId);

                // Checks
                domesticVrpResp2.Should().NotBeNull();
                domesticVrpResp2.Messages.Should().BeEmpty();
                domesticVrpResp2.Data.Should().NotBeNull();

                // DELETE domestic payment
                IFluentResponse domesticVrpResp3 = await requestBuilderNew.VariableRecurringPayments
                    .DomesticVrps
                    .DeleteLocalAsync(domesticVrpId);

                // Checks
                domesticVrpResp3.Should().NotBeNull();
                domesticVrpResp3.Messages.Should().BeEmpty();
            }

            // DELETE auth context


            // DELETE domestic payment consent
            IFluentResponse domesticVrpConsentResp3 = await requestBuilder.VariableRecurringPayments
                .DomesticVrpConsents
                .DeleteLocalAsync(domesticVrpConsentId);

            // Checks
            domesticVrpConsentResp3.Should().NotBeNull();
            domesticVrpConsentResp3.Messages.Should().BeEmpty();
        }
    }
}
