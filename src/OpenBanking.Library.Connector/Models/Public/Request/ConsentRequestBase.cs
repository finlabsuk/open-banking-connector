// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

public class ConsentRequestBase : Base
{
    /// <summary>
    ///     BankProfile used to apply transformations to external API requests.
    /// </summary>
    public BankProfileEnum? BankProfile { get; set; }

    /// <summary>
    ///     Specifies BankRegistration object to use when creating the consent.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public Guid BankRegistrationId { get; set; }

    /// <summary>
    ///     Use existing external API object instead of making external API request.
    ///     The first non-null of ExternalApiObject, ExternalApiRequest, and TemplateRequest (in that order) is used
    ///     and the others are ignored. At least one of these three must be non-null.
    ///     Specifies external Consent created previously using external (bank) API request.
    ///     When non-null, a new external Consent object will
    ///     not be created at the external API.
    /// </summary>
    public ExternalApiConsent? ExternalApiObject { get; set; }

    /// <summary>
    ///     User ID at external API (bank) which may or may not be available via ID token "sub" claim. It can also optionally
    ///     be specified here.
    /// </summary>
    public string? ExternalApiUserId { get; set; }
}
