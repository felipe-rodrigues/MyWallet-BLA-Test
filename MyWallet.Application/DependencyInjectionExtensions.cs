using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyWallet.Application.Services;

namespace MyWallet.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        services.AddSingleton<IWalletEntryService, WalletEntryService>();

        return services;
    }
}