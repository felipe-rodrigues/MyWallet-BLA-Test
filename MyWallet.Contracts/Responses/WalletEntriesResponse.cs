namespace MyWallet.Contracts.Responses;

public class WalletEntriesResponse
{
    public IEnumerable<WalletEntryResponse> Items { get; init; } = Enumerable.Empty<WalletEntryResponse>();
}