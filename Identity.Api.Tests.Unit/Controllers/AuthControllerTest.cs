using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Identity.MyWallet.Api;
using Identity.MyWallet.Api.Controllers;
using Identity.MyWallet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using MyWallet.Application.Models.Responses;
using MyWallet.Application.Services;

namespace Identity.Api.Tests.Unit.Controllers;

public class AuthControllerTest
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly IOptions<JwtSettings> _jwtOptions;
    private readonly JwtSettings _jwtSettings;
    private readonly AuthController _controller;

    public AuthControllerTest()
    {
        _mockUserService = new Mock<IUserService>();
        _jwtSettings = new JwtSettings
        {
            Audience = "TestAudience",
            Issuer = "TestIssuer",
            Key = "12345678901234567890123456789012"
        };
        _jwtOptions = Options.Create(_jwtSettings);
        _controller = new AuthController(_mockUserService.Object, _jwtOptions);
    }

    [Fact]
    public async Task Post_WhenAuthenticationFails_ReturnsUnauthorized()
    {
        var request = new UserLoginRequest
        {
            Email = "wrong@user.com",
            Password = "wrongpass"
        };

        _mockUserService.Setup(x => x.AuthenticateAsync(request.Email, request.Password)).ReturnsAsync(false);

        var result = await _controller.Post(request);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Post_WhenAuthenticationSucceeds_ReturnsOkWithJwt()
    {
        var request = new UserLoginRequest
        {
            Email = "correct@user.com",
            Password = "goodpassword"
        };

        _mockUserService.Setup(x => x.AuthenticateAsync(request.Email, request.Password)).ReturnsAsync(true);

        var result = await _controller.Post(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var loginResponse = Assert.IsType<UserLoginResponse>(okResult.Value);

        Assert.Equal(request.Email, loginResponse.Email);
        Assert.False(string.IsNullOrEmpty(loginResponse.Token));

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

        tokenHandler.ValidateToken(
            loginResponse.Token,
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false
            },
            out var validatedToken);

        Assert.IsType<JwtSecurityToken>(validatedToken);
    }
}