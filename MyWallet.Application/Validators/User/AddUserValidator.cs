using FluentValidation;
using MyWallet.Application.Models;
using MyWallet.Application.Models.Requests;
using MyWallet.Domain.Interfaces.Repositories;

namespace MyWallet.Application.Validators.User;

public class AddUserValidator : AbstractValidator<CreateUserRequest>
{
    
    public AddUserValidator(IUserRepository userRepository)
    {
        
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("User name is required.")
            .MaximumLength(100).WithMessage("User name cannot exceed 100 characters.");
        
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("User email is required.")
            .EmailAddress().WithMessage("User email is not a valid email address.")
            .MaximumLength(255).WithMessage("User email cannot exceed 255 characters.")
            .MustAsync(async (email, cancellation) =>
            {
                var user = await userRepository.GetByEmailAsync(email);
                return user is null;
            })
            .WithMessage("A user with this email already exists.");
        
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        
        RuleFor(user => user.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm Password is required.")
            .Equal(user => user.Password).WithMessage("Confirm Password must match Password.");

    }
}