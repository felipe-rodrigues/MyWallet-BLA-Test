using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWallet.Application.Common.Exceptions;
using MyWallet.Application.Validators.WalletEntry;
using MyWallet.Domain.Entities;
using MyWallet.Web.Middlewares;

namespace MyWallet.Tests.Unit.Web.Middlewares;

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