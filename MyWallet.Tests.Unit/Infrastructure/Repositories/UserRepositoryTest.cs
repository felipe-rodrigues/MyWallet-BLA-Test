using Microsoft.Data.Sqlite;
using MyWallet.Domain.Entities;
using MyWallet.Infrastructure.Data.Configurations;
using MyWallet.Infrastructure.Data.Repositories;

namespace MyWallet.Tests.Unit.Infrastructure.Repositories;

public class UserRepositoryTest : IAsyncLifetime
{
    private IDbConnectionFactory SqliteConnectionFactory { get; set; }
    private SqliteConnection Connection { get; set; }
    private DbInitializer DbInitializer { get; set; }

    public UserRepositoryTest()
    {
        Connection = new SqliteConnection("DataSource=file:memdbTestUser?mode=memory&cache=shared");
        Connection.Open();
        SqliteConnectionFactory = new SqliteConnectionFactory("DataSource=file:memdbTestUser?mode=memory&cache=shared");
        DbInitializer = new DbInitializer(SqliteConnectionFactory);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        var repo = new UserRepository(SqliteConnectionFactory);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "User Test",
            Email = "test@unit.com",
            Hash = "hash123"
        };

        var result = await repo.AddAsync(user);
        Assert.True(result);

        var loaded = await repo.GetByIdAsync(user.Id);
        Assert.NotNull(loaded);
        Assert.Equal("User Test", loaded.Name);
        Assert.Equal("test@unit.com", loaded.Email);
        Assert.Equal("hash123", loaded.Hash);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser()
    {
        var repo = new UserRepository(SqliteConnectionFactory);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Delete User",
            Email = "delete@unit.com",
            Hash = "hashDel"
        };

        await repo.AddAsync(user);
        var deleted = await repo.DeleteAsync(user.Id);
        Assert.True(deleted);

        var loaded = await repo.GetByIdAsync(user.Id);
        Assert.Null(loaded);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserData()
    {
        var repo = new UserRepository(SqliteConnectionFactory);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "NameOld",
            Email = "update@unit.com",
            Hash = "hashUpd"
        };

        await repo.AddAsync(user);
        
        var updated = await repo.UpdateAsync(new User()
        {
            Email = user.Email,
            Hash = user.Hash,
            Name = "NameNew",
            Id = user.Id,
        });
        Assert.True(updated);

        var loaded = await repo.GetByIdAsync(user.Id);
        Assert.Equal("NameNew", loaded.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsers()
    {
        var repo = new UserRepository(SqliteConnectionFactory);
        await repo.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "User All",
            Email = "all@unit.com",
            Hash = "hashAll"
        });

        var all = await repo.GetAllAsync();
        Assert.NotEmpty(all);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnTheCorrectUser()
    {
        var repo = new UserRepository(SqliteConnectionFactory);
        var email = "find@unit.com";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "FindUser",
            Email = email,
            Hash = "hashFind"
        };
        await repo.AddAsync(user);

        var found = await repo.GetByEmailAsync(email);
        Assert.NotNull(found);
        Assert.Equal(user.Id, found.Id);
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