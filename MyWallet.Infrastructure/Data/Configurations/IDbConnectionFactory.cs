using Microsoft.Data.Sqlite;

namespace MyWallet.Infrastructure.Data.Configurations;

public interface IDbConnectionFactory
{
    Task<SqliteConnection> CreateConnectionAsync();
}