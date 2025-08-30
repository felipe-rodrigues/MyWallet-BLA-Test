namespace MyWallet.Domain.Interfaces.Repositories;

public interface IRepository<TEntity>
{
    Task<bool> AddAsync(TEntity walletEntry);
    Task<bool> UpdateAsync(TEntity walletEntry);
    Task<bool> DeleteAsync(Guid id);
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync();
}