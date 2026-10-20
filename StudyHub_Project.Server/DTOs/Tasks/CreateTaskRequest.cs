using StudyHub_Project.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace StudyHub_Project.Server.DTOs.Tasks;

public class CreateTaskRequest
{
    [Required(ErrorMessage = "Введите название задачи")]
    [MaxLength(200, ErrorMessage = "Название слишком длинное")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly Date { get; set; }
    public TaskDifficulty Difficulty { get; set; }
    public string? Category { get; set; }
}