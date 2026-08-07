// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Json;

// VariableRecurringPaymentsModelsPublic aliases the v4.0.1 VRP models, which are used for both real v4.0 and
// real v4.0.1 banks (VariableRecurringPaymentsApiVersion.Version4p0 == VersionPublic). ExternalPaymentTransactionStatus5Code
// must therefore be able to represent every wire value a real v4.0 bank can send, even though some (e.g. CANC) are not
// part of the v4.0.1 spec's own code set for this field. CANC was hand-added to the generated enum for this reason;
// this test guards against that hand-patch being lost on a future NSwag regeneration.
public class VariableRecurringPaymentsStatusTests
{
    [Theory]
    [InlineData("RCVD", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.RCVD)]
    [InlineData("RJCT", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.RJCT)]
    [InlineData("CANC", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.CANC)]
    [InlineData("PDNG", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.PDNG)]
    [InlineData("ACTC", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.ACTC)]
    [InlineData("ACCP", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.ACCP)]
    [InlineData("ACFC", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.ACFC)]
    [InlineData("ACSP", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.ACSP)]
    [InlineData("ACWC", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.ACWC)]
    [InlineData("ACSC", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.ACSC)]
    [InlineData("ACWP", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.ACWP)]
    [InlineData("ACCC", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.ACCC)]
    [InlineData("BLCK", VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code.BLCK)]
    public void RoundTripV4p0DomesticVrpStatusWireValues(
        string wireValue,
        VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code expected)
    {
        // Deserialise exactly as production code does: the Status property is decorated with a plain
        // StringEnumConverter (see Data4.Status in the generated V4p0p1 VRP models).
        var json = $"\"{wireValue}\"";

        var deserialised = JsonConvert
            .DeserializeObject<VariableRecurringPaymentsModelsPublic.ExternalPaymentTransactionStatus5Code>(
                json,
                new StringEnumConverter());

        Assert.Equal(expected, deserialised);

        string reserialised = JsonConvert.SerializeObject(deserialised, new StringEnumConverter());
        Assert.Equal(json, reserialised);
    }
}
