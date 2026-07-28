// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Json;

public class NewEnumMembersDeserialisationTests
{
    [Theory]
    [InlineData("AROI", AccountAndTransactionModelsPublic.ExternalDocumentType1Code.AROI)]
    [InlineData("BOLD", AccountAndTransactionModelsPublic.ExternalDocumentType1Code.BOLD)]
    [InlineData("CMCN", AccountAndTransactionModelsPublic.ExternalDocumentType1Code.CMCN)]
    [InlineData("SOAC", AccountAndTransactionModelsPublic.ExternalDocumentType1Code.SOAC)]
    [InlineData("TSUT", AccountAndTransactionModelsPublic.ExternalDocumentType1Code.TSUT)]
    [InlineData("USAR", AccountAndTransactionModelsPublic.ExternalDocumentType1Code.USAR)]
    [InlineData("VCHR", AccountAndTransactionModelsPublic.ExternalDocumentType1Code.VCHR)]
    public void ExternalDocumentType1Code_NewMembers_DeserialiseWithoutThrowing(
        string jsonValue,
        AccountAndTransactionModelsPublic.ExternalDocumentType1Code expected)
    {
        Deserialise<AccountAndTransactionModelsPublic.ExternalDocumentType1Code>(jsonValue).Should().Be(expected);
    }

    [Theory]
    [InlineData("CRYP", AccountAndTransactionModelsPublic.OBExternalPurpose1Code.CRYP)]
    public void OBExternalPurpose1Code_NewMembers_DeserialiseWithoutThrowing(
        string jsonValue,
        AccountAndTransactionModelsPublic.OBExternalPurpose1Code expected)
    {
        Deserialise<AccountAndTransactionModelsPublic.OBExternalPurpose1Code>(jsonValue).Should().Be(expected);
    }

    [Theory]
    [InlineData("NONE", AccountAndTransactionModelsPublic.OBFrequency2.NONE)]
    [InlineData("SLCT", AccountAndTransactionModelsPublic.OBFrequency2.SLCT)]
    public void OBFrequency2_NewMembers_DeserialiseWithoutThrowing(
        string jsonValue,
        AccountAndTransactionModelsPublic.OBFrequency2 expected)
    {
        Deserialise<AccountAndTransactionModelsPublic.OBFrequency2>(jsonValue).Should().Be(expected);
    }

    [Theory]
    [InlineData("LWMH", AccountAndTransactionModelsPublic.OBFrequency6Code.LWMH)]
    [InlineData("LXMH", AccountAndTransactionModelsPublic.OBFrequency6Code.LXMH)]
    [InlineData("TWYR", AccountAndTransactionModelsPublic.OBFrequency6Code.TWYR)]
    public void OBFrequency6Code_NewMembers_DeserialiseWithoutThrowing(
        string jsonValue,
        AccountAndTransactionModelsPublic.OBFrequency6Code expected)
    {
        Deserialise<AccountAndTransactionModelsPublic.OBFrequency6Code>(jsonValue).Should().Be(expected);
    }

    private static T Deserialise<T>(string jsonValue) =>
        JsonConvert.DeserializeObject<T>($"\"{jsonValue}\"", new StringEnumConverter())!;
}
