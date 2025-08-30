using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWallet.Application.Services;
using MyWallet.Contracts.Requests;
using MyWallet.Web.Mapping;

namespace MyWallet.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WalletEntryController : ControllerBase
{
    private readonly IWalletEntryService _walletEntryService;

    public WalletEntryController(IWalletEntryService walletEntryService)
    {
        _walletEntryService = walletEntryService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWalletEntryRequest request)
    {
        var walletEntry = request.MapToWalletEntry();
        var result = await _walletEntryService.AddAsync(walletEntry);
        return CreatedAtAction(nameof(GetById), new { id = walletEntry.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var walletEntry = await _walletEntryService.GetByIdAsync(id);
        return Ok(walletEntry?.MapToResponse());
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var walletEntries = await _walletEntryService.GetAllAsync();
        return Ok(walletEntries.MapToResponse());
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _walletEntryService.DeleteAsync(id);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalletEntryRequest request)
    {
        var walletEntry = request.MapToWalletEntry(id);
        var result = await _walletEntryService.UpdateAsync(walletEntry);
        return Ok(result.MapToResponse());
    }
}