using MyWallet.Application.Models.Requests;
using MyWallet.Application.Models.Responses;
using MyWallet.Domain.Entities;

namespace MyWallet.Web.Mapping;

public static class ContractsMapping
{
    public static WalletEntry MapToWalletEntry(this CreateWalletEntryRequest request)
    {
        return new WalletEntry()
        {
            Id = Guid.NewGuid(),
            Categories = request.Categories.ToList(),
            Date = request.Date,
            Description = request.Description,
            Value = request.Value,
        };
    }

    public static WalletEntry MapToWalletEntry(this UpdateWalletEntryRequest request, Guid id)
    {
        return new WalletEntry()
        {
            Id = id,
            Categories = request.Categories.ToList(),
            Date = request.Date,
            Description = request.Description,
            Value = request.Value,
        };       
    }

    public static WalletEntryResponse MapToResponse(this WalletEntry walletEntry)
    {
        return new WalletEntryResponse()
        {
            Id = walletEntry.Id,
            Categories = walletEntry.Categories,
            Date = walletEntry.Date,
            Description = walletEntry.Description,
            Value = walletEntry.Value
        };       
    }

    public static WalletEntriesResponse MapToResponse(this IEnumerable<WalletEntry> walletEntries)
    {
        return new WalletEntriesResponse()
        {
            Items = walletEntries.Select(MapToResponse)
        };       
    }
    
}