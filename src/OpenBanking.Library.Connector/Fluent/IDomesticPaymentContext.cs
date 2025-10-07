// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

public interface IDomesticPaymentContext<in TPublicRequest, TPublicResponse, TPublicPaymentDetailsResponse,
    in TCreateParams, in TReadParams> :
    ICreateExternalEntityContext<TPublicRequest, TPublicResponse, TCreateParams>,
    IReadExternalEntityContext<TPublicResponse, TReadParams>,
    IReadExternalEntityPaymentDetailsContext<TPublicPaymentDetailsResponse, TReadParams>
    where TPublicResponse : class
    where TReadParams : ExternalEntityReadParams
    where TCreateParams : ConsentExternalCreateParams
    where TPublicPaymentDetailsResponse : class { }
