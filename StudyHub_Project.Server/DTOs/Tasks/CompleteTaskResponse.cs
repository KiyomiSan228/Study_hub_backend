using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.DTOs.Tasks;

public class CompleteTaskResponse
{

    public required StudyTask Task { get; set; }
    public required Campfire Campfire { get; set; }
    public int LogsAdded { get; set; }
}