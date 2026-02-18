// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Text.RegularExpressions;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

using EndpointDescription = (HttpMethod httpMethod, string pathRegexPattern);

internal enum AispEndpointEnum
{
    GetAccounts,
    GetAccountTransactions
}

public class ReplayClient(BankProfile bankProfile) : IHttpClient
{
    private readonly Dictionary<AispEndpointEnum, EndpointDescription> _aispEndpoints = new()
    {
        [AispEndpointEnum.GetAccounts] = (HttpMethod.Get, @"/accounts"),
        [AispEndpointEnum.GetAccountTransactions] =
            (HttpMethod.Get, @"/accounts/(?<extAccountId>[^/]+)/transactions")
    };

    public Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response;

        // Get AISP base URL
        string aispBaseUrl = bankProfile.GetRequiredAccountAndTransactionApi(false).BaseUrl ?? throw new Exception();

        // Check for matching endpoint 
        AispEndpointEnum? matchingEndpoint = null;
        foreach ((AispEndpointEnum endpoint, EndpointDescription endpointDescription) in _aispEndpoints)
        {
            if (request.Method != endpointDescription.httpMethod ||
                request.RequestUri is null)
            {
                continue;
            }

            var urlString = request.RequestUri.ToString();

            var regexPattern = $"^{aispBaseUrl}{endpointDescription.pathRegexPattern}$";
            Match match = new Regex(
                    regexPattern,
                    RegexOptions.None,
                    TimeSpan.FromMilliseconds(100))
                .Match(urlString);

            if (match.Success)
            {
                matchingEndpoint = endpoint;
                break;
            }
        }

        // Exit if not supported endpoint
        if (matchingEndpoint is null)
        {
            throw new InvalidOperationException(
                $"Replay data not available for this endpoint: " + $"{request.Method} {request.RequestUri}");
        }

        // Produce replay output
        var accountId = "12345ABC";
        var accountSortCodeAccountNumber = "12345612345678";
        var accountName = "SMITH JJ";
        var transactionId = "12345ABC";
        string outputContent = matchingEndpoint switch
        {
            AispEndpointEnum.GetAccounts =>
                $$"""
                  {
                    "Data": {
                      "Account": [
                        {
                          "AccountId": "{{accountId}}",
                          "Currency": "GBP",
                          "AccountType": "Personal",
                          "AccountSubType": "CurrentAccount",
                          "Description": "SELECT ACCOUNT",
                          "Account": [
                            {
                              "SchemeName": "UK.OBIE.SortCodeAccountNumber",
                              "Identification": "{{accountSortCodeAccountNumber}}",
                              "Name": "{{accountName}}"
                            }
                          ]
                        }
                      ]
                    },
                    "Links": {
                      "Self": "{{aispBaseUrl}}/accounts"
                    },
                    "Meta": {}
                  }
                  """,
            AispEndpointEnum.GetAccountTransactions =>
                $$"""
                  {
                    "Data": {
                      "Transaction": [
                        {
                          "AccountId": "{{accountId}}",
                          "TransactionId": "{{transactionId}}",
                          "Amount": {
                            "Amount": "0.30",
                            "Currency": "GBP"
                          },
                          "CreditDebitIndicator": "Debit",
                          "Status": "Booked",
                          "TransactionInformation": "4047 16FEB24 C    THE FISH CHIP SHOPOL                STEVENAGE GB",
                          "BookingDateTime": "2024-02-19T00:00:00Z",
                          "ProprietaryBankTransactionCode": {
                            "Code": "POS"
                          },
                          "Balance": {
                            "Amount": {
                              "Amount": "38.17",
                              "Currency": "GBP"
                            },
                            "CreditDebitIndicator": "Credit",
                            "Type": "Information"
                          }
                        }
                      ]
                    },
                    "Links": {
                      "Self": "{{aispBaseUrl}}/accounts/{{accountId}}/transactions"
                    },
                    "Meta": {}
                  }
                  """,
            _ => throw new ArgumentOutOfRangeException()
        };

        response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(outputContent),
            RequestMessage = request
        };

        return Task.FromResult(response);
    }

    public void Dispose() { }
}
