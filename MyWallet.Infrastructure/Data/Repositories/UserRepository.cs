using MyWallet.Domain.Entities;
using MyWallet.Domain.Interfaces.Repositories;
using MyWallet.Infrastructure.Data.Configurations;

namespace MyWallet.Infrastructure.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    
    public UserRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
        
    }

    public override async Task<bool> AddAsync(User entity)
    {
        await using var connection = await GetConnectionAsync();
        var command = connection.CreateCommand();

        command.CommandText = """
                              insert into users (id, name, email, hash) 
                                  values (@id, @name, @email, @hash);
                              """;
        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@email", entity.Email);
        command.Parameters.AddWithValue("@hash", entity.Hash);
        
        var res = await command.ExecuteNonQueryAsync();
        return res > 0;
    }

    public override async Task<bool> UpdateAsync(User entity)
    {
        await using var connection = await GetConnectionAsync();
        var updateCommand = connection.CreateCommand();
        updateCommand.CommandText = """
                                        update users
                                        set name = @name
                                        where id = @id;
                                    """;
        updateCommand.Parameters.AddWithValue("@id", entity.Id);
        updateCommand.Parameters.AddWithValue("@name", entity.Name);
        var result = await updateCommand.ExecuteNonQueryAsync();
        return result > 0;
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        await using var connection = await GetConnectionAsync();
        var command = connection.CreateCommand();
        command.CommandText = "delete from users where id = @id;";
        command.Parameters.AddWithValue("@id", id);
        var result = await command.ExecuteNonQueryAsync();
        
        return result > 0;
    }

    public override async Task<User?> GetByIdAsync(Guid id)
    {
        await using var connection = await GetConnectionAsync();
        var command = connection.CreateCommand();
        command.CommandText = "select id, name, email, hash from users where id = @id;";
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Hash = reader.GetString(reader.GetOrdinal("hash"))
            };
        }

        return null;

    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        var command = connection.CreateCommand();
        command.CommandText = "select id, name, email, hash from users;";

        var users = new List<User>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Hash = reader.GetString(reader.GetOrdinal("hash"))
            });
        }
        return users;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var connection = await GetConnectionAsync();
        var command = connection.CreateCommand();
        command.CommandText = "select id, name, email, hash from users where email = @email;";
        command.Parameters.AddWithValue("@email", email);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Hash = reader.GetString(reader.GetOrdinal("hash"))
            };
        }

        return null;

    }
}