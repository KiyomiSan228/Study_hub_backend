using System.ComponentModel.DataAnnotations;

namespace StudyHub_Project.Server.DTOs.Notes;

public class CreateNoteRequest
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Напишите хотя бы пару слов")]
    [MaxLength(5000)]
    public string Text { get; set; } = string.Empty;

    public string? Category { get; set; }
}