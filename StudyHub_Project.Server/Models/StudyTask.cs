using System.Text.Json.Serialization;

namespace StudyHub_Project.Server.Models
{
    public class StudyTask
    {
        public int Id { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
        public TaskDifficulty Difficulty { get; set; }
        public string? Category { get; set; }
        public StudyTaskStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
    }
}
