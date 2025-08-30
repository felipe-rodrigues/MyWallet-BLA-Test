using FluentAssertions;
using Identity.MyWallet.Api.Middlewares;
using Microsoft.AspNetCore.Http;
using MyWallet.Application.Common.Exceptions;
using MyWallet.Application.Validators.WalletEntry;
using MyWallet.Domain.Entities;

namespace Identity.Api.Tests.Unit.Middlewares;

public class ValidationMiddlewareTest
{
    [Fact]
    public async Task InvokeAsync_WhenValidationExceptionThrown_Returns400AndValidationProblemDetails()
    {

        var res = await new WalletEntryValidator().ValidateAsync(new WalletEntry()
        {
            Categories = new(),
            Date = DateTime.UtcNow.AddDays(3),
            Id = Guid.NewGuid(),
            Description = "",
            Value = 100
        });
        var context = new DefaultHttpContext();
        var wasCalled = false;

        RequestDelegate next = (ctx) =>
        {
            wasCalled = true;
            throw new ValidationException(res.Errors);
        };

        var middleware = new ValidationMiddleware(next);
        await middleware.InvokeAsync(context);
        
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

}