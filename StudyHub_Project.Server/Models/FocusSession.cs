using System.Text.Json.Serialization;

namespace StudyHub_Project.Server.Models;

public class FocusSession
{
    public int Id { get; set; }

    [JsonIgnore]
    public int UserId { get; set; }

    public int PlannedSeconds { get; set; }

    public int FocusedSeconds { get; set; }

    public FocusSessionStatus Status { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset? EndedAt { get; set; }
}