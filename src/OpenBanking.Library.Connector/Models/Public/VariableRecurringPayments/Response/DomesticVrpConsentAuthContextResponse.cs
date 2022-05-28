// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response
{
    public interface IDomesticVrpConsentAuthContextPublicQuery : IBaseQuery
    {
        public Guid DomesticVrpConsentId { get; }
    }

    /// <summary>
    ///     Response to DomesticVrpConsentAuthContext Read requests.
    /// </summary>
    public class DomesticVrpConsentAuthContextReadResponse : LocalObjectBaseResponse,
        IDomesticVrpConsentAuthContextPublicQuery
    {
        internal DomesticVrpConsentAuthContextReadResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            Guid domesticVrpConsentId) : base(id, created, createdBy, reference)
        {
            DomesticVrpConsentId = domesticVrpConsentId;
        }

        /// <summary>
        ///     Optional list of warning messages from Open Banking Connector.
        /// </summary>
        public IList<string>? Warnings { get; set; }

        public Guid DomesticVrpConsentId { get; }
    }

    /// <summary>
    ///     Response to DomesticVrpConsentAuthContext Create requests.
    /// </summary>
    public class DomesticVrpConsentAuthContextCreateResponse : DomesticVrpConsentAuthContextReadResponse
    {
        internal DomesticVrpConsentAuthContextCreateResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            Guid domesticVrpConsentId,
            string authUrl) : base(id, created, createdBy, reference, domesticVrpConsentId)
        {
            AuthUrl = authUrl;
        }

        public string AuthUrl { get; }

    }
}
