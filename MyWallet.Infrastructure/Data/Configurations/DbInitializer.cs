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
    
    public async Task SeedAsync()
    {
        await SeedUsersTable();
        await SeedWalletEntriesTable();
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

    private async Task SeedUsersTable()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var cmdCheckUsers = connection.CreateCommand();
        cmdCheckUsers.CommandText = "select count(*) from users;";
        var usersCount = Convert.ToInt32(await cmdCheckUsers.ExecuteScalarAsync());

        if (usersCount == 0)
        {
            var insertUserCmd = connection.CreateCommand();
            insertUserCmd.CommandText = """
                                        insert into users (id, name, email, hash) values
                                        (@id1, 'Admin', 'admin@wallet.com', '10000.DeUME4tt9e5AXzRpHvSW2A==.gt719OMI3P1qb+xXGDQV5kxZNYwIMewyXVgYUznxdn0='),
                                        (@id2, 'Test User', 'test@wallet.com', '10000.L/28ZfMv3TCRL8jy/RzvOQ==.8yKOAjbM5zI37rSTAHfk78xZ9GjH/dkk21GXdziTXW8=');
                                        """;
            insertUserCmd.Parameters.AddWithValue("@id1", Guid.NewGuid().ToString());
            insertUserCmd.Parameters.AddWithValue("@id2", Guid.NewGuid().ToString());
            await insertUserCmd.ExecuteNonQueryAsync();
        }
    }
    
    private async Task SeedWalletEntriesTable()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var cmdCheckEntries = connection.CreateCommand();
        cmdCheckEntries.CommandText = "select count(*) from wallet_entries;";
        var entryCount = Convert.ToInt32(await cmdCheckEntries.ExecuteScalarAsync());

        if (entryCount == 0)
        {
            var entryId = Guid.NewGuid().ToString();
            var entryId2 = Guid.NewGuid().ToString();
            var insertEntryCmd = connection.CreateCommand();
            insertEntryCmd.CommandText = """
                                         insert into wallet_entries (id, description, value, date) 
                                         values (@id, 'Month Salaray', 3000, @date),
                                         (@id2, 'Family Lunch', -200, @date)
                                         """;
            insertEntryCmd.Parameters.AddWithValue("@id", entryId);
            insertEntryCmd.Parameters.AddWithValue("@id2", entryId2);
            insertEntryCmd.Parameters.AddWithValue("@date", DateTime.UtcNow.ToString("yyyy-MM-dd"));

            await insertEntryCmd.ExecuteNonQueryAsync();

            var insertCategoryCmd = connection.CreateCommand();
            insertCategoryCmd.CommandText = """
                                            insert into categories (entryid, name) values 
                                            (@entryId, 'Earnings'),
                                            (@entryId, 'Salary'),
                                            (@entryId2, 'Lunch'),
                                            (@entryId2, 'Family')
                                            """;
            insertCategoryCmd.Parameters.AddWithValue("@entryId", entryId);
            insertCategoryCmd.Parameters.AddWithValue("@entryId2", entryId2);

            await insertCategoryCmd.ExecuteNonQueryAsync();
        }
    }
}