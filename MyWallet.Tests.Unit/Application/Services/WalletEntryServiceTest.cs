using FluentAssertions;
using Moq;
using MyWallet.Application.Common.Exceptions;
using MyWallet.Application.Services;
using MyWallet.Application.Validators.WalletEntry;
using MyWallet.Domain.Entities;
using MyWallet.Domain.Interfaces.Repositories;

namespace MyWallet.Tests.Unit.Application.Services;

public class WalletEntryServiceTest
{
    private readonly Mock<IWalletEntryRepository> _repositoryMock;
    private readonly WalletEntryService _service;

    public WalletEntryServiceTest()
    {
        _repositoryMock = new Mock<IWalletEntryRepository>();
        var validator = new WalletEntryValidator();
        _service = new WalletEntryService(_repositoryMock.Object, validator);
    }
    
    [Fact]
    public async Task AddAsync_WhenValidatorFails_ThrowsValidationException()
    {
        var walletEntry = new WalletEntry
        {
            Id = Guid.NewGuid(),
            Description = null,
            Value = 0,
            Date = DateTime.Now,
            Categories = new List<string>()
        };
        
        Func<Task> act = () => _service.AddAsync(walletEntry);

        await act.Should().ThrowAsync<ValidationException>();
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<WalletEntry>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_WhenValid_CallsRepositoryAndReturnsResult()
    {
        var walletEntry = new WalletEntry
        {
            Id = Guid.NewGuid(),
            Description = "desc",
            Value = 10,
            Date = DateTime.Now,
            Categories = new List<string>()
        };
        
        _repositoryMock
            .Setup(x => x.AddAsync(walletEntry))
            .ReturnsAsync(true).Verifiable();

        var result = await _service.AddAsync(walletEntry);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_CallsRepositoryAndReturnsWalletEntry()
    {
        var walletEntry = new WalletEntry
        {
            Id = Guid.NewGuid(),
            Description = "desc",
            Value = 10,
            Date = DateTime.Now,
            Categories = new List<string>()
        };

        _repositoryMock.Setup(x => x.UpdateAsync(walletEntry))
            .ReturnsAsync(true).Verifiable();

        var result = await _service.UpdateAsync(walletEntry);

        result.Should().Be(walletEntry);
        _repositoryMock.Verify(x => x.UpdateAsync(walletEntry), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryAndReturnsResult()
    {
        var id = Guid.NewGuid();

        _repositoryMock.Setup(x => x.DeleteAsync(id))
            .ReturnsAsync(true).Verifiable();

        var result = await _service.DeleteAsync(id);

        result.Should().BeTrue();
        _repositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity()
    {
        var id = Guid.NewGuid();
        var entity = new WalletEntry
        {
            Id = id,
            Description = "desc",
            Value = 10,
            Date = DateTime.Now,
            Categories = new List<string>()
        };
        _repositoryMock.Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(entity).Verifiable();

        var result = await _service.GetByIdAsync(id);

        result.Should().Be(entity);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEntries()
    {
        var walletEntries = new List<WalletEntry>
        {
            new WalletEntry
            {
                Id = Guid.NewGuid(),
                Description = "desc",
                Value = 10,
                Date = DateTime.Now,
                Categories = new List<string>()
            }
        };

        _repositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(walletEntries).Verifiable();

        var result = await _service.GetAllAsync();

        result.Should().BeEquivalentTo(walletEntries);
    }

}