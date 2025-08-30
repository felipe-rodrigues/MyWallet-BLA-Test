using Microsoft.Extensions.DependencyInjection;
using MyWallet.Domain.Interfaces.Repositories;
using MyWallet.Infrastructure.Data.Configurations;
using MyWallet.Infrastructure.Data.Repositories;

namespace MyWallet.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,string connectionString)
    {
        return services.AddDatabase(connectionString)
            .AddSingleton<IWalletEntryRepository, WalletEntryRepository>()
            .AddSingleton<IUserRepository, UserRepository>();
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<DbInitializer>();
        services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
        return services;
    }
}