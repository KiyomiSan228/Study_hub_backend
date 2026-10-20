using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudyHub_Project.Server.Extensions;
using StudyHub_Project.Server.Models;
using StudyHub_Project.Server.Services;

namespace StudyHub_Project.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/campfire")]
public class CampfireController : ControllerBase
{
    private readonly CampfireService _campfireService;


    public CampfireController(
        CampfireService campfireService)
    {
        _campfireService =
            campfireService;
    }


    [HttpGet]
    public ActionResult<Campfire> Get()
    {
        return Ok(
            _campfireService.Get(
                User.GetRequiredUserId()));
    }
}