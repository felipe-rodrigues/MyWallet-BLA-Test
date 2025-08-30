using Identity.MyWallet.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyWallet.Application.Models.Requests;
using MyWallet.Application.Services;

namespace Identity.Api.Tests.Unit.Controllers;

public class UserControllerTest
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _controller;

    public UserControllerTest()
    {
        _mockUserService = new Mock<IUserService>();
        _controller = new UserController(_mockUserService.Object);
    }

    [Fact]
    public async Task CreateUser_WhenAddAsyncReturnsTrue_ReturnsOk()
    {
        var request = new CreateUserRequest
        {
            Name = "TestUser",
            Email = "test@example.com",
            Password = "12345678",
            ConfirmPassword = "12345678"
        };

        _mockUserService.Setup(s => s.AddAsync(request)).ReturnsAsync(true);

        var result = await _controller.CreateUser(request);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task CreateUser_WhenAddAsyncReturnsFalse_ReturnsBadRequest()
    {
        var request = new CreateUserRequest
        {
            Name = "User2",
            Email = "fail@example.com",
            Password = "password",
            ConfirmPassword = "password"
        };

        _mockUserService.Setup(s => s.AddAsync(request)).ReturnsAsync(false);

        var result = await _controller.CreateUser(request);

        Assert.IsType<BadRequestResult>(result);
    }
}