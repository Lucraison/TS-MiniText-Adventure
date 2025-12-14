using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace TextAdventure.Security;

public static class CmsDecryptor
{
    // ECHTE try: nooit crashen, geen errors als plaintext teruggeven
    public static bool TryDecryptToString(string encPath, string pfxPath, string pfxPassword, out string plaintext)
    {
        plaintext = "";

        try
        {
            if (!File.Exists(encPath)) return false;
            if (!File.Exists(pfxPath)) return false;

            // Onze .enc is S/MIME tekst -> base64 payload eruit halen
            var smimeText = File.ReadAllText(encPath);
            var base64 = ExtractBase64Block(smimeText);
            if (base64 is null) return false;

            var cmsBytes = Convert.FromBase64String(base64);

            // Cert + private key openen met password (DIT password wordt jouw derived key)
            var cert = new X509Certificate2(
                pfxPath,
                pfxPassword,
                X509KeyStorageFlags.EphemeralKeySet);

            var env = new EnvelopedCms();
            env.Decode(cmsBytes);
            env.Decrypt(new X509Certificate2Collection(cert));

            plaintext = System.Text.Encoding.UTF8.GetString(env.ContentInfo.Content);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string? ExtractBase64Block(string smime)
    {
        // Zoek de lege lijn die headers van payload scheidt
        var idx = smime.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        var skip = 4;

        if (idx < 0)
        {
            idx = smime.IndexOf("\n\n", StringComparison.Ordinal);
            skip = 2;
        }

        if (idx < 0) return null;

        var payload = smime[(idx + skip)..]
            .Replace("\r", "")
            .Trim();

        return string.IsNullOrWhiteSpace(payload) ? null : payload;
    }
}
