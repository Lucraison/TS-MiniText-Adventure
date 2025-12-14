using System.Security.Cryptography;
using System.Text;

namespace TextAdventure.Security;

public static class CryptoHelpers
{
    public static string Sha256Hex(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash); // bv. "A1B2..."
    }
}
