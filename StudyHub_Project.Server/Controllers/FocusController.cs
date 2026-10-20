using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudyHub_Project.Server.DTOs.Focus;
using StudyHub_Project.Server.Extensions;
using StudyHub_Project.Server.Models;
using StudyHub_Project.Server.Services;

namespace StudyHub_Project.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/focus")]
public class FocusController : ControllerBase
{
    private readonly FocusService _focusService;


    public FocusController(
        FocusService focusService)
    {
        _focusService =
            focusService;
    }


    [HttpGet("sessions")]
    public ActionResult<
        IReadOnlyCollection<FocusSession>>
        GetSessions()
    {
        return Ok(
            _focusService.GetAll(
                User.GetRequiredUserId()));
    }


    [HttpPost("sessions")]
    public ActionResult<FocusSession> Start(
        [FromBody] StartFocusRequest request)
    {
        var session =
            _focusService.Start(
                User.GetRequiredUserId(),
                request);


        return StatusCode(
            StatusCodes.Status201Created,
            session);
    }


    [HttpPost("sessions/{id:int}/finish")]
    public ActionResult<FocusSession> Finish(
        int id,
        [FromBody] FinishFocusRequest request)
    {
        return Ok(
            _focusService.Finish(
                User.GetRequiredUserId(),
                id,
                request));
    }
}