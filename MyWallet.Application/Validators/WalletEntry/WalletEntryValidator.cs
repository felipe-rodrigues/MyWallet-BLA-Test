using FluentValidation;

namespace MyWallet.Application.Validators.WalletEntry;

public class WalletEntryValidator : AbstractValidator<Domain.Entities.WalletEntry>
{
    public WalletEntryValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(200)
            .WithMessage("Description must have at most 200 characters.");

        RuleFor(x => x.Value)
            .NotEqual(0)
            .WithMessage("Value must not be zero.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Date cannot be in the future.");
    }
}