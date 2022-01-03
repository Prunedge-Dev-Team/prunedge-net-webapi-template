using Application.Contracts;
using Application.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IServiceManager _service;

    public AuthController(IServiceManager service)
    {
        _service = service;
    }

    /// <summary>
    /// Registers new user
    /// </summary>
    /// <param name="userForRegistrationDto"></param>
    /// <returns>New User</returns>

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistrationDto)
    {
        var result = await _service.AuthenticationService.RegisterUser(userForRegistrationDto);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        return StatusCode(201);
    }

    /// <summary>
    /// Logs in user with email and password
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Jwt Token</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserForLoginDto user)
    {
        if (!await _service.AuthenticationService.ValidateUser(user))
            return Unauthorized();
        
        var tokenDto = await _service.AuthenticationService.CreateToken(populateExp: true);

        return Ok(tokenDto);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="tokenDto"></param>
    /// <returns></returns>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
    {
        var token = await _service.AuthenticationService.RefreshToken(tokenDto);
        return Ok(token);
    }
}