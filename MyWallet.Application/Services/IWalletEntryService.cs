using MyWallet.Domain.Entities;

namespace MyWallet.Application.Services;

public interface IWalletEntryService
{
    Task<bool> AddAsync(WalletEntry walletEntry);
    Task<WalletEntry> UpdateAsync(WalletEntry walletEntry);
    Task<bool> DeleteAsync(Guid id);
    Task<WalletEntry?> GetByIdAsync(Guid id);
    Task<IEnumerable<WalletEntry>> GetAllAsync();
}