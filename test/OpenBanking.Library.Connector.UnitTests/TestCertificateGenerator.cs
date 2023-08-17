// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;
using PemWriter = Org.BouncyCastle.OpenSsl.PemWriter;
using X509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests;

public static class TestCertificateGenerator
{
    public const string DefaultSignatureAlgorithm = "SHA256withRSA";

    public static AsymmetricCipherKeyPair GenerateKeyPair()
    {
        var keypairgen = new RsaKeyPairGenerator();
        var parameters = new KeyGenerationParameters(
            new SecureRandom(new CryptoApiRandomGenerator()),
            1024);
        keypairgen.Init(parameters);

        AsymmetricCipherKeyPair? keypair = keypairgen.GenerateKeyPair();

        return keypair;
    }

    public static X509Certificate GenerateCertificate(
        AsymmetricCipherKeyPair keyPair,
        string certName,
        string? signatureAlgorithm)
    {
        signatureAlgorithm = signatureAlgorithm ?? DefaultSignatureAlgorithm;

        var gen = new X509V3CertificateGenerator();

        var cn = new X509Name("CN=" + certName);
        BigInteger? serial = BigInteger.ProbablePrime(120, new Random());

        gen.SetSerialNumber(serial);
        gen.SetSubjectDN(cn);
        gen.SetIssuerDN(cn);
        gen.SetNotAfter(DateTime.MaxValue);
        gen.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
        gen.SetPublicKey(keyPair.Public);

        var sf = new Asn1SignatureFactory(
            signatureAlgorithm,
            keyPair.Private);

        Org.BouncyCastle.X509.X509Certificate? r = gen.Generate(sf);

        return DotNetUtilities.ToX509Certificate(r);
    }


    public static (X509Certificate, RSA) GenerateCertificateWithPrivateKey(
        AsymmetricCipherKeyPair keyPair,
        string certName,
        string signatureAlgorithm = DefaultSignatureAlgorithm)
    {
        var gen = new X509V3CertificateGenerator();

        var cn = new X509Name("CN=" + certName);
        BigInteger? serial = BigInteger.ProbablePrime(120, new Random());

        gen.SetSerialNumber(serial);
        gen.SetSubjectDN(cn);
        gen.SetIssuerDN(cn);
        gen.SetNotAfter(DateTime.MaxValue);
        gen.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
        gen.SetPublicKey(keyPair.Public);


        var sf = new Asn1SignatureFactory(
            signatureAlgorithm,
            keyPair.Private);

        Org.BouncyCastle.X509.X509Certificate? r = gen.Generate(sf);

        var cert = DotNetUtilities.ToX509Certificate(r);

        var certV2 = new X509Certificate2(cert);

        var akp2 = (RsaPrivateCrtKeyParameters) keyPair.Private;

        var rsaParameters = DotNetUtilities.ToRSAParameters(akp2);

        var privateKey = RSA.Create(rsaParameters);

        return (certV2, privateKey);
    }

    public static X509Certificate ToX509V2Cert(X509Certificate cert) => new(cert);

    public static X509Certificate GenerateRandomCert(string name)
    {
        AsymmetricCipherKeyPair keyPair = GenerateKeyPair();

        X509Certificate originCert = GenerateCertificate(
            keyPair,
            name,
            null);

        X509Certificate cert = ToX509V2Cert(originCert);

        return cert;
    }


    public static string GetPemTextFromPublicKey(AsymmetricCipherKeyPair keys)
    {
        TextWriter textWriter = new StringWriter();
        var pemWriter = new PemWriter(textWriter);
        pemWriter.WriteObject(keys.Public);
        pemWriter.Writer.Flush();
        return textWriter.ToString()!;
    }

    public static string GetPemTextFromCertificate(X509Certificate cert)
    {
        TextWriter textWriter = new StringWriter();
        var pemWriter = new PemWriter(textWriter);

        PemObjectGenerator pog = new PemObject("CERTIFICATE", cert.GetRawCertData());

        pemWriter.WriteObject(pog);
        pemWriter.Writer.Flush();
        return textWriter.ToString()!;
    }
}
