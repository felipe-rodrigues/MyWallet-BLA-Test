namespace MyWallet.Domain.Entities;

public class User : IEntity
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Hash { get; init; } = string.Empty;
}