using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyWallet.Application.Models.Requests;
using MyWallet.Application.Models.Responses;
using MyWallet.Application.Services;
using MyWallet.Domain.Entities;
using MyWallet.Domain.Interfaces.Services;
using MyWallet.Web.Controllers;
using MyWallet.Web.Mapping;

namespace MyWallet.Tests.Unit.Web.Controllers;

public class WalletControllerTest
{
    private readonly Mock<IWalletEntryService> _serviceMock;
    private readonly WalletEntryController _controller;

    public WalletControllerTest()
    {
        _serviceMock = new Mock<IWalletEntryService>();
        _controller = new WalletEntryController(_serviceMock.Object);
    }

    [Fact]
    public async Task Create_Returns_CreatedAtAction()
    {
        var request = new CreateWalletEntryRequest
        {
            Description = "Some desc",
            Value = 123,
            Date = DateTime.Today,
            Categories = new List<string> { "Test1", "Test2" }
        };

        var expectedEntity = request.MapToWalletEntry();
        _serviceMock.Setup(x => x.AddAsync(It.IsAny<WalletEntry>())).ReturnsAsync(true);

        var result = await _controller.Create(request);

        var actionRes = result as CreatedAtActionResult;
        actionRes.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetById_Returns_OkWithTypeOfResponse()
    {
        var id = Guid.NewGuid();
        var entity = new WalletEntry
        {
            Id = id,
            Description = "X",
            Value = 15,
            Date = DateTime.Today,
            Categories = new List<string> { "C1" }
        };
        _serviceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);

        var result = await _controller.GetById(id);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        var walletRes = okResult.Value as WalletEntryResponse;
        walletRes.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetAll_Returns_OkWithListOfResponse()
    {
        var walletEntries = new List<WalletEntry>
        {
            new WalletEntry
            {
                Id = Guid.NewGuid(),
                Description = "Y",
                Value = 10,
                Date = DateTime.Today,
                Categories = new List<string> { "Test" }
            }
        };
        _serviceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(walletEntries);

        var result = await _controller.GetAll();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult?.Value.Should().NotBeNull();
        var walletRes = okResult.Value as WalletEntriesResponse;
        walletRes.Should().NotBeNull();
        walletRes.Items.Should().HaveCount(1);
    }
    
    
    [Fact]
    public async Task Delete_Returns_OkWithNoContent()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(true);

        var result = await _controller.Delete(id);
        result.Should().BeAssignableTo<OkResult>();
    }
    
    [Fact]
    public async Task Update_Returns_OkWithTypeResponse()
    {
        var id = Guid.NewGuid();
        var request = new UpdateWalletEntryRequest
        {
            Description = "Updated",
            Value = 150,
            Date = DateTime.Today,
            Categories = new List<string> { "C2" }
        };
        var entity = request.MapToWalletEntry(id);

        _serviceMock.Setup(x => x.UpdateAsync(It.IsAny<WalletEntry>())).ReturnsAsync(entity);

        var result = await _controller.Update(id, request);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        var walletRes = okResult.Value as WalletEntryResponse;
        walletRes.Should().NotBeNull();
        walletRes.Should().BeEquivalentTo(request);
    }
}