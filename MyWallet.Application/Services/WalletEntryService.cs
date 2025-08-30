using MyWallet.Application.Common.Exceptions;
using MyWallet.Application.Validators.WalletEntry;
using MyWallet.Domain.Entities;
using MyWallet.Domain.Interfaces.Repositories;

namespace MyWallet.Application.Services;

public class WalletEntryService : IWalletEntryService
{
    private readonly IWalletEntryRepository _walletEntryRepository;
    private readonly WalletEntryValidator _walletEntryValidator;

    public WalletEntryService(IWalletEntryRepository walletEntryRepository, WalletEntryValidator walletEntryValidator)
    {
        _walletEntryRepository = walletEntryRepository;
        _walletEntryValidator = walletEntryValidator;
    }

    public async Task<bool> AddAsync(WalletEntry walletEntry)
    {
        var result = await _walletEntryValidator.ValidateAsync(walletEntry);
        
        if(!result.IsValid)
            throw new ValidationException(result.Errors);
        
        return await _walletEntryRepository.AddAsync(walletEntry);
    } 

    public async Task<WalletEntry?> UpdateAsync(WalletEntry walletEntry)
    {
        var result = await _walletEntryRepository.UpdateAsync(walletEntry);
        return !result ? null : walletEntry;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _walletEntryRepository.DeleteAsync(id);
    }

    public async Task<WalletEntry?> GetByIdAsync(Guid id)
    {
        var walletEntry = await _walletEntryRepository.GetByIdAsync(id);
        return walletEntry;
    }

    public async Task<IEnumerable<WalletEntry>> GetAllAsync()
    {
        var walletEntries = await _walletEntryRepository.GetAllAsync();
        return walletEntries;
    }
}