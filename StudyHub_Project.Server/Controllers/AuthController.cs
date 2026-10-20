using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudyHub_Project.Server.DTOs.Auth;
using StudyHub_Project.Server.Exceptions;
using StudyHub_Project.Server.Extensions;
using StudyHub_Project.Server.Services;

namespace StudyHub_Project.Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;


    public AuthController(
        AuthService authService)
    {
        _authService =
            authService;
    }


    [AllowAnonymous]
    [HttpPost("register")]
    public ActionResult<AuthResponse> Register(
        [FromBody] RegisterRequest request)
    {
        return Ok(
            _authService.Register(
                request));
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<AuthResponse> Login(
        [FromBody] LoginRequest request)
    {
        return Ok(
            _authService.Login(
                request));
    }


    [Authorize]
    [HttpGet("me")]
    public ActionResult<UserResponse> Me()
    {
        return Ok(
            _authService.GetCurrentUser(
                User.GetRequiredUserId()));
    }


    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var header =
            Request.Headers.Authorization
                .ToString();


        if (!header.StartsWith(
                "Bearer ",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ApiException(
                401,
                "Сессия недействительна");
        }


        var token =
            header["Bearer ".Length..]
                .Trim();


        _authService.Logout(
            token);


        return NoContent();
    }
}