using System.Security.Cryptography;
using System.Text;
using DeviceOwnership.Core.Interfaces;

namespace DeviceOwnership.Infrastructure.Services;

public class EncryptionService : IEncryptionService
{
    private readonly string _encryptionKey;

    public EncryptionService(string encryptionKey)
    {
        _encryptionKey = encryptionKey;
    }

    public Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32).Substring(0, 32));
        aes.IV = new byte[16];

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return Task.FromResult(Convert.ToBase64String(msEncrypt.ToArray()));
    }

    public Task<string> DecryptAsync(string encryptedText, CancellationToken cancellationToken = default)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32).Substring(0, 32));
        aes.IV = new byte[16];

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText));
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return Task.FromResult(srDecrypt.ReadToEnd());
    }

    public string Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public string HashWithSalt(string input, string salt)
    {
        return Hash(input + salt);
    }

    public bool VerifyHash(string input, string salt, string hash)
    {
        var computedHash = HashWithSalt(input, salt);
        return computedHash == hash;
    }

    public string GenerateRandomCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
