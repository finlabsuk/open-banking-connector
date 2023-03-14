// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

public class AccountAndTransactionApiHiddenProperties
{
    public AccountAndTransactionApiVersion? ApiVersion { get; set; }

    public string? BaseUrl { get; set; }
}

/// <summary>
///     Describes <see cref="PaymentInitiationApi" /> hidden properties object in bankProfileHiddenProperties.json
///     Properties are nullable versions (since optional) of those in <see cref="PaymentInitiationApi" />.
/// </summary>
public class PaymentInitiationApiHiddenProperties
{
    public PaymentInitiationApiVersion? ApiVersion { get; set; }

    public string? BaseUrl { get; set; }
}

/// <summary>
///     Describes <see cref="VariableRecurringPaymentsApi" /> hidden properties object in bankProfileHiddenProperties.json
///     Properties are nullable versions (since optional) of those in <see cref="VariableRecurringPaymentsApi" />.
/// </summary>
public class VariableRecurringPaymentsApiHiddenProperties
{
    public VariableRecurringPaymentsApiVersion? ApiVersion { get; set; }

    public string? BaseUrl { get; set; }
}

/// <summary>
///     Describes <see cref="BankProfile" /> hidden properties object in bankProfileHiddenProperties.json.
///     Properties are nullable versions (since optional) of those in <see cref="BankProfile" />.
/// </summary>
public class BankProfileHiddenProperties
{
    public string? IssuerUrl { get; set; }

    public string? FinancialId { get; set; }

    public DynamicClientRegistrationApiVersion? DefaultClientRegistrationApiVersion { get; set; }

    public AccountAndTransactionApiHiddenProperties? AccountAndTransactionApi { get; set; }

    public PaymentInitiationApiHiddenProperties? PaymentInitiationApi { get; set; }

    public VariableRecurringPaymentsApiHiddenProperties? VariableRecurringPaymentsApi { get; set; }

    public string? Extra1 { get; set; }

    public string? Extra2 { get; set; }
}
