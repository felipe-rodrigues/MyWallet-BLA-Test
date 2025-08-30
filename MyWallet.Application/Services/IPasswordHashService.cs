namespace MyWallet.Domain.Interfaces.Services;

public interface IPasswordHashService
{
    string Hash(string password);

    (bool verified, bool needsUpgrade) CheckPassword(string hash, string password);
}