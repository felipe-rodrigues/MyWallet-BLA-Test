using System.Security.Cryptography;
using MyWallet.Domain.Interfaces.Services;

namespace MyWallet.Application.Services;

public class PasswordHashService : IPasswordHashService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;

    public (bool verified, bool needsUpgrade) CheckPassword(string hash, string password)
    {
        var parts = hash.Split('.');

        if (parts.Length != 3)
        {
            throw new Exception();
        }

        var iterations = Convert.ToInt32(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);

        var needsUpgrade = iterations != Iterations;

        using (var algorithm = new Rfc2898DeriveBytes(
                   password,
                   salt,
                   iterations,
                   HashAlgorithmName.SHA256
                   ))
        {
            var keyToCheck = algorithm.GetBytes(KeySize);

            var verified = keyToCheck.SequenceEqual(key);

            return (verified, needsUpgrade);
        }

    }

    public string Hash(string password)
    {
        using (var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations,HashAlgorithmName.SHA256))
        {
            var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
            var salt = Convert.ToBase64String(algorithm.Salt);

            return $"{Iterations}.{salt}.{key}";
        }
    }
}