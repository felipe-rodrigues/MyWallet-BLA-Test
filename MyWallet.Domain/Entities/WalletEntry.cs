namespace MyWallet.Domain.Entities;

public class WalletEntry : IEntity
{
    public required Guid Id { get; init; }
    public required string Description { get; set; }
    public required decimal Value { get; set; }
    public required DateTime Date { get; set; }
    public required List<string> Categories { get; set; } = new();
}