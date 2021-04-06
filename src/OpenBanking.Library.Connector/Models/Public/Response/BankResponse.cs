// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBankPublicQuery
    {
        string IssuerUrl { get; }
        string FinancialId { get; }
        string? Name { get; }
        Guid Id { get; }
    }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class BankGetLocalResponse : IBankPublicQuery
    {
        internal BankGetLocalResponse(string issuerUrl, string xFapiFinancialId, string? name, Guid id)
        {
            IssuerUrl = issuerUrl;
            FinancialId = xFapiFinancialId;
            Name = name;
            Id = id;
        }

        public string IssuerUrl { get; }

        public string FinancialId { get; }

        public string? Name { get; }

        public Guid Id { get; }
    }

    /// <summary>
    ///     Response to Post
    /// </summary>
    public class BankPostResponse : BankGetLocalResponse
    {
        internal BankPostResponse(string issuerUrl, string xFapiFinancialId, string? name, Guid id) : base(
            issuerUrl,
            xFapiFinancialId,
            name,
            id) { }
    }
}
