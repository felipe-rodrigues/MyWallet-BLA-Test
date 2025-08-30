using Microsoft.Data.Sqlite;
using MyWallet.Domain.Interfaces.Repositories;
using MyWallet.Infrastructure.Data.Configurations;

namespace MyWallet.Infrastructure.Data.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    private readonly IDbConnectionFactory _connectionFactory;

    protected BaseRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    protected Task<SqliteConnection> GetConnectionAsync() => _connectionFactory.CreateConnectionAsync();

    public abstract Task<bool> AddAsync(T entity);
    public abstract Task<bool> UpdateAsync(T entity);
    public abstract Task<bool> DeleteAsync(Guid id);
    public abstract Task<T?> GetByIdAsync(Guid id);
    public abstract Task<IEnumerable<T>> GetAllAsync();
}