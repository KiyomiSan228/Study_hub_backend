namespace StudyHub_Project.Server.Data;

public class LoginAttemptState
{
    public int Fails { get; set; }

    public DateTimeOffset? LockedUntil { get; set; }
}