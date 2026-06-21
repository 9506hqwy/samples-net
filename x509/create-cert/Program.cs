using System.Net;
using System.Security.Cryptography;
using X509 = System.Security.Cryptography.X509Certificates;

try
{
    var hostname = Dns.GetHostName();
    using var privKey = CreatePrivateKey(2048);
    var csr = CreateRequest(privKey, hostname);
    var pfx = CreatePfx(csr, hostname);

    Export(pfx);
}
#pragma warning disable CA1031
catch (Exception e)
#pragma warning restore CA1031
{
    await Console.Error.WriteLineAsync($"{e}").ConfigureAwait(false);
}

static RSACryptoServiceProvider CreatePrivateKey(int keySize)
{
    if (OperatingSystem.IsWindows())
    {
        var cps = new CspParameters
        {
            Flags =
                CspProviderFlags.UseArchivableKey |
                CspProviderFlags.UseMachineKeyStore |
                CspProviderFlags.UseDefaultKeyContainer,
        };

        return new RSACryptoServiceProvider(keySize, cps);
    }
    else
    {
        return new RSACryptoServiceProvider(keySize);
    }
}

static X509.CertificateRequest CreateRequest(
    RSACryptoServiceProvider privKey,
    string dnsName)
{
    var csr = new X509.CertificateRequest(
        $"CN={dnsName}",
        privKey,
        HashAlgorithmName.SHA256,
        RSASignaturePadding.Pkcs1);

    var constants = new X509.X509BasicConstraintsExtension(false, false, 0, false);
    csr.CertificateExtensions.Add(constants);

    var keyUsage = new X509.X509KeyUsageExtension(
        X509.X509KeyUsageFlags.DataEncipherment |
        X509.X509KeyUsageFlags.KeyEncipherment,
        false);
    csr.CertificateExtensions.Add(keyUsage);

    // サーバー認証
    var serverOid = new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") };
    csr.CertificateExtensions.Add(new X509.X509EnhancedKeyUsageExtension(serverOid, false));

    var nameBuilder = new X509.SubjectAlternativeNameBuilder();
    nameBuilder.AddDnsName(dnsName);
    csr.CertificateExtensions.Add(nameBuilder.Build());

    return csr;
}

static X509.X509Certificate2 CreatePfx(
    X509.CertificateRequest csr,
    string friendlyName)
{
    var notBefore = DateTimeOffset.UtcNow;
    var notAfter = notBefore.AddYears(10);
    var pfx = csr.CreateSelfSigned(notBefore, notAfter);
    if (OperatingSystem.IsWindows())
    {
        pfx.FriendlyName = friendlyName;
    }

    return pfx;
}

static void Export(X509.X509Certificate2 cer)
{
    var bytes = cer.Export(X509.X509ContentType.Pkcs12, "password");
    File.WriteAllBytes("self.pfx", bytes);
}
