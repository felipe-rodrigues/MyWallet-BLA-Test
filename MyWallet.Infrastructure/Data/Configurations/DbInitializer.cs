using Microsoft.Data.Sqlite;

namespace MyWallet.Infrastructure.Data.Configurations;

public class DbInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DbInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task InitializeAsync()
    {
        await CreateWalletEntriesTable();
        await CreateCategoriesTable();
        await CreateUsersTable();
    }

    private async Task CreateWalletEntriesTable()
    {
        using var connection  = await _connectionFactory.CreateConnectionAsync();
        var command = connection.CreateCommand();
        command.CommandText =
            """
            create table if not exists wallet_entries (
                id text primary key, 
                description text not null, 
                value real not null, 
                date text not null 
            )
            """;
    
        await command.ExecuteNonQueryAsync();
    }

    private async Task CreateCategoriesTable()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var command = connection.CreateCommand();
        command.CommandText =
            """
            create table if not exists categories (
                entryid text not null,
                name text not null,
                primary key (entryid, name),
                foreign key (entryid) references wallet_entries(id)
            )
            """;

        await command.ExecuteNonQueryAsync();
    }

    private async Task CreateUsersTable()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var command = connection.CreateCommand();
        command.CommandText =
            """
            create table if not exists users (
                id text primary key, 
                name text not null, 
                email text not null, 
                hash text not null
            )
            """;

        await command.ExecuteNonQueryAsync();
    }
}