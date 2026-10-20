using System.Text.Json.Serialization;

namespace StudyHub_Project.Server.Models;

public class Campfire
{
    [JsonIgnore]
    public int UserId { get; set; }
    public int Logs { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}