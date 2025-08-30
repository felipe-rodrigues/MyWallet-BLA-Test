using MyWallet.Application.Common.Exceptions;
using MyWallet.Application.Models;
using MyWallet.Application.Models.Requests;
using MyWallet.Application.Validators.User;
using MyWallet.Domain.Entities;
using MyWallet.Domain.Interfaces.Repositories;
using MyWallet.Domain.Interfaces.Services;

namespace MyWallet.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly AddUserValidator _addUserValidator;

    public UserService(IUserRepository userRepository, IPasswordHashService passwordHashService, AddUserValidator addUserValidator)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _addUserValidator = addUserValidator;
    }

    public async Task<bool> AddAsync(CreateUserRequest userRequest)
    {
        var result = await _addUserValidator.ValidateAsync(userRequest);
        
        if(!result.IsValid)
            throw new ValidationException(result.Errors);

        var hash = _passwordHashService.Hash(userRequest.Password);
        return await _userRepository.AddAsync(new User()
        {
            Name = userRequest.Name,
            Hash = hash,
            Email = userRequest.Email,
            Id = Guid.NewGuid()
        });
    }

    public async Task<bool> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        
        if(user is null)
            throw new Exception("User not found");
        
        var check = _passwordHashService.CheckPassword(user.Hash, password);
        return check.verified;
    }
}