using MyWallet.Domain.Entities;
using MyWallet.Domain.Interfaces.Repositories;
using MyWallet.Infrastructure.Data.Configurations;

namespace MyWallet.Infrastructure.Data.Repositories;

public class WalletEntryRepository : BaseRepository<WalletEntry>, IWalletEntryRepository
{
    public WalletEntryRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
        
    }

    public override async Task<bool> AddAsync(WalletEntry entity)
    {
        await using var connection = await GetConnectionAsync();
        await using var transaction = connection.BeginTransaction();
        var command = connection.CreateCommand();

        command.CommandText = """
                              insert into wallet_entries (id, description, value, date) 
                                  values (@id, @description, @value, @date);
                              """;
        command.Parameters.AddWithValue("@id", entity.Id.ToString());
        command.Parameters.AddWithValue("@description", entity.Description);
        command.Parameters.AddWithValue("@value", entity.Value);
        command.Parameters.AddWithValue("@date", entity.Date);
        
        var res = await command.ExecuteNonQueryAsync();

        if (res > 0)
        {
            foreach (var entityCategory in entity.Categories)
            {
                var categoriesCommand = connection.CreateCommand();
                categoriesCommand.CommandText = """
                                                insert into categories (entryId, name) values (@id, @name);
                                                """;
                categoriesCommand.Parameters.AddWithValue("@id", entity.Id.ToString());
                categoriesCommand.Parameters.AddWithValue("@name", entityCategory);
                await categoriesCommand.ExecuteNonQueryAsync();
            }
        }
        
        transaction.Commit();
        
        return res > 0;
    }

    public override async Task<bool> UpdateAsync(WalletEntry entity)
    {
        await using var connection = await GetConnectionAsync();
        await using var transaction = connection.BeginTransaction();
        
        var updateCommand = connection.CreateCommand();
        updateCommand.CommandText = """
            update wallet_entries
            set description = @description, value = @value, date = @date
            where id = @id;
        """;
        updateCommand.Parameters.AddWithValue("@id", entity.Id.ToString());
        updateCommand.Parameters.AddWithValue("@description", entity.Description);
        updateCommand.Parameters.AddWithValue("@value", entity.Value);
        updateCommand.Parameters.AddWithValue("@date", entity.Date);

        var result = await updateCommand.ExecuteNonQueryAsync();

        if (result > 0)
        {
            var deleteCategoriesCommand = connection.CreateCommand();
            deleteCategoriesCommand.CommandText = "delete from categories where entryId = @id;";
            deleteCategoriesCommand.Parameters.AddWithValue("@id", entity.Id.ToString());
            await deleteCategoriesCommand.ExecuteNonQueryAsync();
        
            foreach (var category in entity.Categories)
            {
                var insertCategoryCommand = connection.CreateCommand();
                insertCategoryCommand.CommandText = "insert into categories (entryId, name) values (@id, @name);";
                insertCategoryCommand.Parameters.AddWithValue("@id", entity.Id.ToString());
                insertCategoryCommand.Parameters.AddWithValue("@name", category);
                await insertCategoryCommand.ExecuteNonQueryAsync();
            }
            transaction.Commit();
        }
        
        return result > 0;
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        await using var connection = await GetConnectionAsync();
        await using var transaction = connection.BeginTransaction();
        
        var deleteCategoriesCmd = connection.CreateCommand();
        deleteCategoriesCmd.CommandText = "delete from categories where entryId = @id;";
        deleteCategoriesCmd.Parameters.AddWithValue("@id", id.ToString());
        await deleteCategoriesCmd.ExecuteNonQueryAsync();

        var deleteEntryCmd = connection.CreateCommand();
        deleteEntryCmd.CommandText = "delete from wallet_entries where id = @id;";
        deleteEntryCmd.Parameters.AddWithValue("@id", id.ToString());
        var affected = await deleteEntryCmd.ExecuteNonQueryAsync();

        transaction.Commit();
        return affected > 0;
    }

    public override async Task<WalletEntry?> GetByIdAsync(Guid id)
    {
        await using var connection = await GetConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = """
                              select we.id, we.description, we.value, we.date, GROUP_CONCAT(c.name, ',') as categories
                              from wallet_entries we LEFT JOIN categories c on we.id = c.entryId
                              where we.id = @id COLLATE NOCASE
                              group by we.id, we.description, we.value, we.date
                              """;
        command.Parameters.AddWithValue("@id", id.ToString());

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var walletEntry = new WalletEntry
            {
                Id = reader.GetGuid(0),
                Description = reader.GetString(1),
                Value = reader.GetDecimal(2),
                Date = reader.GetDateTime(3),
                Categories = reader.IsDBNull(4)
                    ? new List<string>()
                    : reader.GetString(4).Split(',').ToList()
            };
            return walletEntry;
        }
        return null;
    }

    public override async Task<IEnumerable<WalletEntry>> GetAllAsync()
    {
        var walletEntries = new List<WalletEntry>();

        await using var connection = await GetConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = """
                                  select we.id, we.description, we.value, we.date, GROUP_CONCAT(c.name, ',') as categories
                                  from wallet_entries we 
                                  left join categories c on we.id = c.entryId
                                  group by we.id, we.description, we.value, we.date
                                  order by we.date desc;
                              """;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var walletEntry = new WalletEntry
            {
                Id = reader.GetGuid(0),
                Description = reader.GetString(1),
                Value = reader.GetDecimal(2),
                Date = reader.GetDateTime(3),
                Categories = reader.IsDBNull(4) 
                    ? new List<string>() 
                    : reader.GetString(4).Split(',').ToList()
            };

            walletEntries.Add(walletEntry);
        }

        return walletEntries;

    }
}