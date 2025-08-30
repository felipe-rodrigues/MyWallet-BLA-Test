using Microsoft.Data.Sqlite;
using MyWallet.Domain.Entities;
using MyWallet.Infrastructure.Data.Configurations;
using MyWallet.Infrastructure.Data.Repositories;

namespace MyWallet.Tests.Unit.Infrastructure.Repositories;

public class WalletEntryRepositoryTest : IAsyncLifetime
{
    private IDbConnectionFactory SqliteConnectionFactory { get; set; }
    private SqliteConnection Connection { get; set; }
    private DbInitializer DbInitializer { get; set; }

    public WalletEntryRepositoryTest()
    {
       Connection = new SqliteConnection("DataSource=file:memdb1?mode=memory&cache=shared");
       Connection.Open();
       SqliteConnectionFactory = new SqliteConnectionFactory("DataSource=file:memdb1?mode=memory&cache=shared");
       DbInitializer = new DbInitializer(SqliteConnectionFactory);
    }
    
    [Fact]
    public async Task AddAsync_ShouldAddEntry()
    {
        var repo = new WalletEntryRepository(SqliteConnectionFactory);
        var entry = new WalletEntry
        {
            Id = Guid.NewGuid(),
            Description = "Teste",
            Value = 20,
            Date = DateTime.Today,
            Categories = new List<string> { "Food", "Leisure" }
        };

        var result = await repo.AddAsync(entry);
        Assert.True(result);

        var loaded = await repo.GetByIdAsync(entry.Id);
        Assert.NotNull(loaded);
        Assert.Equal("Teste", loaded.Description);
        Assert.Equal(20, loaded.Value);
        Assert.Equal(entry.Categories.Count, loaded.Categories.Count);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldDeleteEntry()
    {
        var repo = new WalletEntryRepository(SqliteConnectionFactory);
        var entry = new WalletEntry
        {
            Id = Guid.NewGuid(),
            Description = "Delete Test",
            Value = 100,
            Date = DateTime.Today,
            Categories = new List<string> { "Bills" }
        };

        await repo.AddAsync(entry);
        var deleted = await repo.DeleteAsync(entry.Id);
        Assert.True(deleted);

        var loaded = await repo.GetByIdAsync(entry.Id);
        Assert.Null(loaded);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateData()
    {
        var repo = new WalletEntryRepository(SqliteConnectionFactory);
        var entry = new WalletEntry
        {
            Id = Guid.NewGuid(),
            Description = "To Update",
            Value = 10,
            Date = DateTime.Today,
            Categories = new List<string> { "Old" }
        };

        await repo.AddAsync(entry);

        entry.Description = "Updated!";
        entry.Value = 999;
        entry.Categories = new List<string> { "NewCategory" };

        var updated = await repo.UpdateAsync(entry);
        Assert.True(updated);

        var loaded = await repo.GetByIdAsync(entry.Id);
        Assert.Equal("Updated!", loaded.Description);
        Assert.Equal(999, loaded.Value);
        Assert.Contains("NewCategory", loaded.Categories);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEntries()
    {
        var repo = new WalletEntryRepository(SqliteConnectionFactory);
        await repo.AddAsync(new WalletEntry
        {
            Id = Guid.NewGuid(),
            Description = "AllTest",
            Value = 1,
            Date = DateTime.Today,
            Categories = new List<string> { "Test" }
        });

        var all = await repo.GetAllAsync();
        Assert.NotEmpty(all);
    }


    public async Task InitializeAsync()
    {
        await DbInitializer.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await Connection.DisposeAsync();
    }
}