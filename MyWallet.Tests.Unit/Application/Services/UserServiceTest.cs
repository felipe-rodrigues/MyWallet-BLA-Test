using Moq;
using MyWallet.Application.Common.Exceptions;
using MyWallet.Application.Models.Requests;
using MyWallet.Application.Services;
using MyWallet.Application.Validators.User;
using MyWallet.Domain.Entities;
using MyWallet.Domain.Interfaces.Repositories;
using MyWallet.Domain.Interfaces.Services;

namespace MyWallet.Tests.Unit.Application.Services;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHashService> _passwordHashServiceMock;
    private readonly AddUserValidator _userValidator;
    private readonly UserService _userService;

    public UserServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHashServiceMock = new Mock<IPasswordHashService>();
        _userValidator = new AddUserValidator(_userRepositoryMock.Object);

        _userService = new UserService(
            _userRepositoryMock.Object,
            _passwordHashServiceMock.Object,
            _userValidator
        );
    }

    [Fact]
    public async Task AddAsync_WhenUserIsAddedSuccessfully_ShouldReturnTrue()
    {
        var userRequest = new CreateUserRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var hash = "hashedpassword";
        
        _passwordHashServiceMock.Setup(s => s.Hash(userRequest.Password)).Returns(hash);

        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(true);

        var result = await _userService.AddAsync(userRequest);

        Assert.True(result);
        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == userRequest.Email && u.Hash == hash)), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenUserRequestIsInvalid_ShouldThrowValidationException()
    {
        var userRequest = new CreateUserRequest
        {
            Name = "Invalid User",
            Email = "invalid",
            Password = "123",
            ConfirmPassword = "123"
        };
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.AddAsync(userRequest));
        Assert.Equal(2, exception.Errors.Count);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_WhenUserRepositoryAddFails_ShouldReturnFalse()
    {
        var userRequest = new CreateUserRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var hash = "hashedpassword";
        
        _passwordHashServiceMock.Setup(s => s.Hash(userRequest.Password)).Returns(hash);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(false);

        var result = await _userService.AddAsync(userRequest);

        Assert.False(result);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenEmailAlreadyExist_ShouldThrowValidationException()
    {
        var userRequest = new CreateUserRequest
        {
            Name = "IUser",
            Email = "email@alreadyexist",
            Password = "1234567",
            ConfirmPassword = "1234567"
        };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(userRequest.Email)).ReturnsAsync((User?)new User()).Verifiable();
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.AddAsync(userRequest));
        Assert.Equal(1, exception.Errors.Count);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenCredentialsAreValid_ShouldReturnTrue()
    {
        var email = "user@example.com";
        var password = "correctpassword";
        var user = new User { Email = email, Hash = "hashedpassword" };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
        _passwordHashServiceMock.Setup(s => s.CheckPassword(user.Hash, password)).Returns((true, false));

        var result = await _userService.AuthenticateAsync(email, password);

        Assert.True(result);
        _userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
        _passwordHashServiceMock.Verify(s => s.CheckPassword(user.Hash, password), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenPasswordIsIncorrect_ShouldReturnFalse()
    {
        var email = "user@example.com";
        var password = "incorrectpassword";
        var user = new User { Email = email, Hash = "hashedpassword" };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
        _passwordHashServiceMock.Setup(s => s.CheckPassword(user.Hash, password)).Returns((false, false));

        var result = await _userService.AuthenticateAsync(email, password);

        Assert.False(result);
        _userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
        _passwordHashServiceMock.Verify(s => s.CheckPassword(user.Hash, password), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldThrowException_WhenUserIsNotFound()
    {
        var email = "nonexistent@example.com";
        var password = "anypassword";

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _userService.AuthenticateAsync(email, password));
        Assert.Equal("User not found", exception.Message);
        _userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
        _passwordHashServiceMock.Verify(s => s.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

}