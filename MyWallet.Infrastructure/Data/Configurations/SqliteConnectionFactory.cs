using Microsoft.Data.Sqlite;

namespace MyWallet.Infrastructure.Data.Configurations;

public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    
    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<SqliteConnection> CreateConnectionAsync()
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}