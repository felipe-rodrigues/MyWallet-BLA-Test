namespace MyWallet.Application.Models.Responses;

public class WalletEntryResponse
{
    public Guid Id { get; init; }
    public required string Description { get; init; }
    public required decimal Value { get; init; }
    public required DateTime Date { get; init; }
    public required IEnumerable<string> Categories { get; init; } = Enumerable.Empty<string>();
}