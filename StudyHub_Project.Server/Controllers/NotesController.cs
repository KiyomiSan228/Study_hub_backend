using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudyHub_Project.Server.DTOs.Notes;
using StudyHub_Project.Server.Extensions;
using StudyHub_Project.Server.Models;
using StudyHub_Project.Server.Services;

namespace StudyHub_Project.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/notes")]
public class NotesController : ControllerBase
{
    private readonly NoteService _noteService;


    public NotesController(
        NoteService noteService)
    {
        _noteService =
            noteService;
    }


    [HttpGet]
    public ActionResult<
        IReadOnlyCollection<Note>>
        Get()
    {
        return Ok(
            _noteService.GetAll(
                User.GetRequiredUserId()));
    }


    [HttpPost]
    public ActionResult<Note> Create(
        [FromBody] CreateNoteRequest request)
    {
        var note =
            _noteService.Create(
                User.GetRequiredUserId(),
                request);


        return StatusCode(
            StatusCodes.Status201Created,
            note);
    }


    [HttpPut("{id:int}")]
    public ActionResult<Note> Update(
        int id,
        [FromBody] UpdateNoteRequest request)
    {
        return Ok(
            _noteService.Update(
                User.GetRequiredUserId(),
                id,
                request));
    }


    [HttpDelete("{id:int}")]
    public IActionResult Delete(
        int id)
    {
        _noteService.Delete(
            User.GetRequiredUserId(),
            id);


        return NoContent();
    }
}