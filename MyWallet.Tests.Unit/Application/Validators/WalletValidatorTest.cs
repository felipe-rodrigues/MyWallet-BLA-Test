using FluentAssertions;
using MyWallet.Application.Validators.WalletEntry;
using MyWallet.Domain.Entities;

namespace MyWallet.Tests.Unit.Application.Validators;

public class WalletValidatorTest
{
    [Fact]
    public void IsValid_ForDescriptionNull_ReturnsFalse()
    {
        var validator = new WalletEntryValidator();

        var res = validator.Validate(new WalletEntry()
        {
            Id = Guid.NewGuid(),
            Description = null,
            Value = 100,
            Date = DateTime.Today,
            Categories = new List<string>() { "Food" }
        });
        
        res.IsValid.Should().BeFalse();
        res.Errors.Should().ContainSingle(x => x.PropertyName == "Description");
    }
    
    [Fact]
    public void IsValid_ForDescriptionTooLong_ReturnsFalse()
    {
        var validator = new WalletEntryValidator();

        var res = validator.Validate(new WalletEntry()
        {
            Id = Guid.NewGuid(),
            Description = new string('A', 201),
            Value = 100,
            Date = DateTime.Today,
            Categories = new List<string>() { "Food" }
        });
        
        res.IsValid.Should().BeFalse();
        res.Errors.Should().Contain(x => x.PropertyName == "Description");
    }
    
    [Fact]
    public void IsValid_ForValueZero_ReturnsFalse()
    {
        var validator = new WalletEntryValidator();

        var res = validator.Validate(new WalletEntry()
        {
            Id = Guid.NewGuid(),
            Description = "Valid",
            Value = 0,
            Date = DateTime.Today,
            Categories = new List<string>() { "Food" }
        });

        res.IsValid.Should().BeFalse();
        res.Errors.Should().Contain(x => x.PropertyName == "Value");
    }

    [Fact]
    public void IsValid_ForDateInFuture_ReturnsFalse()
    {
        var validator = new WalletEntryValidator();

        var res = validator.Validate(new WalletEntry()
        {
            Id = Guid.NewGuid(),
            Description = "Valid",
            Value = 10,
            Date = DateTime.UtcNow.AddDays(1),
            Categories = new List<string>() { "Food" }
        });

        res.IsValid.Should().BeFalse();
        res.Errors.Should().Contain(x => x.PropertyName == "Date");
    }

    [Fact]
    public void IsValid_ForValidWalletEntry_ReturnsTrue()
    {
        var validator = new WalletEntryValidator();

        var res = validator.Validate(new WalletEntry()
        {
            Id = Guid.NewGuid(),
            Description = "Valid description",
            Value = 10,
            Date = DateTime.UtcNow.AddDays(-1),
            Categories = new List<string>() { "Food" }
        });

        res.IsValid.Should().BeTrue();
        res.Errors.Should().BeEmpty();
    }
    
}