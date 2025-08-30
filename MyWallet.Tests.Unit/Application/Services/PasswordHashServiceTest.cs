using MyWallet.Application.Services;

namespace MyWallet.Tests.Unit.Application.Services;

public class PasswordHashServiceTest
{
    private readonly PasswordHashService _service;

    public PasswordHashServiceTest()
    {
        _service = new PasswordHashService();
    }

    [Fact]
    public void Hash_WhenCalled_ReturnsHashContainingThreeParts()
    {
        var hash = _service.Hash("mypassword");
        var parts = hash.Split('.');
        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void CheckPassword_WhenPasswordIsCorrect_ReturnsVerifiedTrueAndNeedsUpgradeFalse()
    {
        var password = "securepassword";
        var hash = _service.Hash(password);
        var (verified, needsUpgrade) = _service.CheckPassword(hash, password);

        Assert.True(verified);
        Assert.False(needsUpgrade);
    }

    [Fact]
    public void CheckPassword_WhenPasswordIsIncorrect_ReturnsVerifiedFalse()
    {
        var hash = _service.Hash("original");
        var (verified, needsUpgrade) = _service.CheckPassword(hash, "diffpass");
        Assert.False(verified);
    }
    
}