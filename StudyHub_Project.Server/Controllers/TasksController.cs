using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHub_Project.Server.DTOs.Tasks;
using StudyHub_Project.Server.Exceptions;
using StudyHub_Project.Server.Extensions;
using StudyHub_Project.Server.Models;
using StudyHub_Project.Server.Services;
using System.Globalization;

namespace StudyHub_Project.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;


    public TasksController(
        TaskService taskService)
    {
        _taskService =
            taskService;
    }


    [HttpGet]
    public ActionResult<
        IReadOnlyCollection<StudyTask>>
        Get(
            [FromQuery] string? date)
    {
        var userId =
            User.GetRequiredUserId();


        if (string.IsNullOrWhiteSpace(
                date))
        {
            return Ok(
                _taskService.GetAll(
                    userId));
        }


        if (!DateOnly.TryParseExact(
                date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate))
        {
            throw new ApiException(
                400,
                "Некорректная дата.");
        }


        return Ok(
            _taskService.GetByDate(
                userId,
                parsedDate));
    }


    [HttpPost]
    public ActionResult<StudyTask> Create(
        [FromBody] CreateTaskRequest request)
    {
        var task =
            _taskService.Create(
                User.GetRequiredUserId(),
                request);


        return StatusCode(
            StatusCodes.Status201Created,
            task);
    }


    [HttpPut("{id:int}")]
    public ActionResult<StudyTask> Update(
        int id,
        [FromBody] UpdateTaskRequest request)
    {
        return Ok(
            _taskService.Update(
                User.GetRequiredUserId(),
                id,
                request));
    }


    [HttpDelete("{id:int}")]
    public IActionResult Delete(
        int id)
    {
        _taskService.Delete(
            User.GetRequiredUserId(),
            id);


        return NoContent();
    }


    [HttpPost("{id:int}/complete")]
    public ActionResult<CompleteTaskResponse>
        Complete(
            int id)
    {
        return Ok(
            _taskService.Complete(
                User.GetRequiredUserId(),
                id));
    }


    [HttpPost("{id:int}/fail")]
    public ActionResult<StudyTask> Fail(
        int id)
    {
        return Ok(
            _taskService.Fail(
                User.GetRequiredUserId(),
                id));
    }
}