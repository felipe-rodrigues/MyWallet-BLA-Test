namespace MyWallet.Application.Models.Responses;

public class ValidationFailureResponse
{
    public required IEnumerable<ValidationResponse> Errors { get; init; } = Enumerable.Empty<ValidationResponse>();
}

public class ValidationResponse
{
    public required string PropertyName { get; init; } = string.Empty;
    public required string Message { get; init; } 
}