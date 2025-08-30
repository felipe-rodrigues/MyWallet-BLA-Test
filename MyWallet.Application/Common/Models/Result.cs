namespace MyWallet.Application.Common.Models;

public readonly struct Result<TValeu, TError>
{
    private readonly TValeu? _value;
    private readonly TError? _error;

    public Result(TValeu value)
    {
        IsError = false;
        _value = value;
        _error = default;
    }

    public Result(TError error)
    {
        IsError = true;
        _error = error;
        _value = default;
    }

    public bool IsError { get; }
    public bool IsSuccess => !IsError;

    public static implicit operator Result<TValeu, TError>(TValeu value) => new(value);

    public static implicit operator Result<TValeu, TError>(TError error) => new(error);

    public TResult Match<TResult>(Func<TValeu, TResult> success,
        Func<TError, TResult> failure) =>
        !IsError ? success(_value!) : failure(_error!);
}