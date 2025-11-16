namespace DeviceOwnership.Core.Interfaces;

public interface IEncryptionService
{
    Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default);
    Task<string> DecryptAsync(string encryptedText, CancellationToken cancellationToken = default);
    string Hash(string input);
    string HashWithSalt(string input, string salt);
    bool VerifyHash(string input, string salt, string hash);
    string GenerateRandomCode(int length = 6);
}
