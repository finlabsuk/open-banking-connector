// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using Jose;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security;

public enum JwtType
{
    Jwt,
    Jose
}

public static class JwtFactory
{
    public static string CreateJwt(
        Dictionary<string, object> headers,
        string payloadJson,
        string signingKey,
        JwsAlgorithm? jwsAlgorithm)
    {
        signingKey.ArgNotNull(nameof(signingKey));

        var rsa = RSA.Create();
        try
        {
            CertificateFactories.ImportPrivateKey(signingKey, ref rsa);

            string result = JWT.Encode(
                payloadJson,
                rsa,
                jwsAlgorithm ?? JwsAlgorithm.PS256,
                headers);
            return result;
        }
        finally
        {
            rsa.Dispose();
        }
    }

    private static string GetJwtTypeString(JwtType jwtType) =>
        jwtType switch
        {
            JwtType.Jwt => "JWT",
            JwtType.Jose => "JOSE",
            _ => throw new ArgumentOutOfRangeException(nameof(jwtType), jwtType, null)
        };

    public static Dictionary<string, object> GetDefaultJwtHeaders(string signingId, JwtType? jwtType = null)
    {
        var jwtHeaders = new Dictionary<string, object> { ["kid"] = signingId.ArgNotNull(nameof(signingId)) };
        if (jwtType is not null)
        {
            jwtHeaders.Add("typ", GetJwtTypeString(jwtType.Value));
        }

        return jwtHeaders;
    }
}
