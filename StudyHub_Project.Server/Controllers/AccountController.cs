using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudyHub_Project.Server.DTOs.Auth;
using StudyHub_Project.Server.Extensions;
using StudyHub_Project.Server.Services;

namespace StudyHub_Project.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;


    public AccountController(
        AccountService accountService)
    {
        _accountService =
            accountService;
    }


    [HttpPut]
    public ActionResult<UserResponse> Update(
        [FromBody] UpdateAccountRequest request)
    {
        return Ok(
            _accountService.UpdateAccount(
                User.GetRequiredUserId(),
                request));
    }


    [HttpPut("password")]
    public IActionResult ChangePassword(
        [FromBody] ChangePasswordRequest request)
    {
        _accountService.ChangePassword(
            User.GetRequiredUserId(),
            request);


        return NoContent();
    }


    [HttpDelete]
    public IActionResult Delete(
        [FromBody] DeleteAccountRequest request)
    {
        _accountService.DeleteAccount(
            User.GetRequiredUserId(),
            request);


        return NoContent();
    }
}