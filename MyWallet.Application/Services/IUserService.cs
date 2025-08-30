using MyWallet.Application.Models;
using MyWallet.Application.Models.Requests;

namespace MyWallet.Application.Services;

public interface IUserService
{
    Task<bool> AddAsync(CreateUserRequest userRequest);
    Task<bool> AuthenticateAsync(string email,string password);
}