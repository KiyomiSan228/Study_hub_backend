using System.Text.Json.Serialization;

namespace StudyHub_Project.Server.Models;

public class Note
{
    public int Id { get; set; }

    [JsonIgnore]
    public int UserId { get; set; }

    public string? Title { get; set; }

    public string Text { get; set; } = string.Empty;

    public string? Category { get; set; }

    public bool IsPinned { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}