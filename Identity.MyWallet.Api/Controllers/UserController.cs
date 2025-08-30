using Microsoft.AspNetCore.Mvc;
using MyWallet.Application.Models.Requests;
using MyWallet.Application.Services;

namespace Identity.MyWallet.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest userRequestRequest)
    {
        var result = await _userService.AddAsync(userRequestRequest);
        
        if(!result)
            return BadRequest();
        
        return Ok();
    }
}