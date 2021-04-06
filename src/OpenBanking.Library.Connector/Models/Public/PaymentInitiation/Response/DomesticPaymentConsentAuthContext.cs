// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public interface IDomesticPaymentConsentAuthContextPublicQuery { }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class DomesticPaymentConsentAuthContextGetLocalResponse : IDomesticPaymentConsentAuthContextPublicQuery { }

    /// <summary>
    ///     Response to Get, Post
    /// </summary>
    public class DomesticPaymentConsentAuthContextPostResponse : DomesticPaymentConsentAuthContextGetLocalResponse
    {
        public DomesticPaymentConsentAuthContextPostResponse(string authUrl)
        {
            AuthUrl = authUrl;
        }

        public string AuthUrl { get; }
    }
}
